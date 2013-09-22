using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Service;
using Wlq.Web.Fliters;
using Wlq.Web.Models;

namespace Wlq.Web.Controllers
{
	public class AdminController : BaseController
	{
		#region Members

		private UserInfo _adminUser;

		protected UserInfo AdminUser
		{
			get
			{
				if (_adminUser == null)
				{
					if (CurrentUser != null && CurrentUser.Role > (int)RoleLevel.Normal)
					{
						_adminUser = CurrentUser;
					}
				}

				return _adminUser;
			}
		}

		#endregion

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Index()
		{
			var totalNumber = 0;

			var model = new AdminIndexModel
			{
				ActivityList = PostService.GetPostsByType(true, PostType.Activity, true, 1, 8, out totalNumber),
				CourseList = PostService.GetPostsByType(true, PostType.Course, true, 1, 8, out totalNumber),
				VenueList = PostService.GetPostsByType(true, PostType.Venue, true, 1, 8, out totalNumber)
			};

			return View(model);
		}

		#region User & Group

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult AdminManagement()
		{
			var model = new AdminManagementModel();
			var isSuperAdmin = AdminUser.Role == (int)RoleLevel.SuperAdmin;

			model.Departments = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role)
				.Where(g => g.ParentGroupId == 0);

			if (!isSuperAdmin && model.Departments != null && model.Departments.Count() > 0)
			{
				model.Circles = UserGroupService.GetGroupsByParent(model.Departments.First().Id);
			}

			ViewBag.SuperAdmin = isSuperAdmin;

			return View(model);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult GroupManagement()
		{
			var groups = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role)
				.Where(g => g.ParentGroupId == 0);

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Group(long groupId, long parentGroupId)
		{
			if (groupId > 0 && CurrentUser.Role != (int)RoleLevel.SuperAdmin && !UserGroupService.IsManagerInGroup(CurrentUserId, groupId))
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			var parentGroup = UserGroupService.GetGroup(parentGroupId, false);

			ViewBag.ParentGroupName = parentGroup == null ? "无" : parentGroup.Name;

			var group = groupId > 0
				? UserGroupService.GetGroup(groupId, false)
				: new GroupInfo { ParentGroupId = parentGroupId };

			if (group == null || group.ParentGroupId != parentGroupId)
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			CommonService.CleanTempFile(CurrentUserId);

			return View(group);
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SaveGroup(GroupInfo groupModel)
		{
			if (groupModel.Id > 0 && CurrentUser.Role != (int)RoleLevel.SuperAdmin && !UserGroupService.IsManagerInGroup(CurrentUserId, groupModel.Id))
			{
				return RedirectToAction("GroupManagement", "Admin");
			}
			else if (groupModel.Id == 0 && groupModel.ParentGroupId == 0 && CurrentUser.Role != (int)RoleLevel.SuperAdmin)
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			var group = groupModel.Id > 0
				? UserGroupService.GetGroup(groupModel.Id, false)
				: new GroupInfo();

			if (group == null)
			{
				return AlertAndRedirect("保存失败(该组不存在)", "/Admin/GroupManagement");
			}

			group.Name = groupModel.Name;
			group.ParentGroupId = groupModel.ParentGroupId;
			group.GroupType = groupModel.ParentGroupId == 0 ? (int)GroupType.Department : (int)GroupType.Circle;
			group.Logo = groupModel.Logo;
			group.Address = groupModel.Address;
			group.Phone = groupModel.Phone;
			group.Email = groupModel.Email;
			group.WorkTime = groupModel.WorkTime;
			group.Introduction = groupModel.Introduction;

			if (group.Id == 0)
			{
				if (!UserGroupService.AddGroup(group))
				{
					return AlertAndRedirect("保存失败(添加新组失败)", "/Admin/GroupManagement");
				}
			}
			else
			{
				if (!UserGroupService.UpdateGroup(group))
				{
					return AlertAndRedirect("保存失败(更新组织失败)", "/Admin/GroupManagement");
				}
			}

			CommonService.SaveGroupLogo(CurrentUserId, group.Id);

			return AlertAndRedirect("保存成功", "/Admin/GroupManagement");
		}

		#endregion

		#region Venue

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult VenueManagement()
		{
			var groups = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role);

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult VenueGroup(long id)
		{
			var venueGroup = id > 0
				? PostService.GetVenueGroup(id)
				: new VenueGroupInfo();

			if (venueGroup == null)
			{
				return RedirectToAction("VenueManagement", "Admin");
			}

			var model = new AdminVenueGroupModel();

			model.VenueGroup = venueGroup;
			model.Venues = id > 0 ? PostService.GetVenuesByVenueGroup(id) : null;
			model.Groups = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role);

			return View(model);
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SaveVenueGroup(VenueGroupInfo venueGroupModel)
		{
			var venueGroup = venueGroupModel.Id > 0
				? PostService.GetVenueGroup(venueGroupModel.Id)
				: new VenueGroupInfo();

			if (venueGroup == null)
			{
				return AlertAndRedirect("保存失败(场馆不存在)", "/Admin/VenueManagement");
			}

			venueGroup.Name = venueGroupModel.Name;
			venueGroup.VenueType = venueGroupModel.VenueType;
			venueGroup.GroupId = venueGroupModel.GroupId;

			if (venueGroup.Id == 0)
			{
				if (!PostService.AddVenueGroup(venueGroup))
				{
					return AlertAndRedirect("保存失败(添加场馆失败)", "/Admin/VenueManagement");
				}
			}
			else
			{
				if (!PostService.UpdateVenueGroup(venueGroup))
				{
					return AlertAndRedirect("保存失败(更新场馆失败)", "/Admin/VenueManagement");
				}
			}

			return AlertAndRedirect("保存成功", "/Admin/VenueManagement");
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Venue(long id, long venueGroupId)
		{
			var venueGroup = PostService.GetVenueGroup(venueGroupId);

			if (venueGroup == null)
			{
				return RedirectToAction("VenueManagement", "Admin");
			}

			var venueConfig = PostService.GetVenueConfigs(id);

			ViewBag.GroupId = venueGroup.GroupId;
			ViewBag.VenueGroupId = venueGroup.Id;
			ViewBag.VenueGroupName = venueGroup.Name;
			ViewBag.VenueConfig = venueConfig.ToJson();

			var venue = id > 0
				? PostService.GetVenue(id)
				: new VenueInfo();

			if (venue == null)
			{
				return RedirectToAction("VenueManagement", "Admin");
			}

			return View(venue);
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SaveVenue(VenueInfo venueModel, string config)
		{
			var venue = venueModel.Id > 0
				? PostService.GetVenue(venueModel.Id)
				: new VenueInfo();

			if (venue == null)
			{
				return AlertAndRedirect("保存失败(场地不存在)", "/Admin/VenueManagement");
			}

			venue.Name = venueModel.Name;
			venue.Address = venueModel.Address;
			venue.GroupId = venueModel.GroupId;
			venue.VenueGroupId = venueModel.VenueGroupId;

			if (venue.Id == 0)
			{
				if (!PostService.AddVenue(venue))
				{
					return AlertAndRedirect("保存失败(添加场地失败)", "/Admin/VenueManagement");
				}
			}
			else
			{
				if (!PostService.UpdateVenue(venue))
				{
					return AlertAndRedirect("保存失败(更新场地失败)", "/Admin/VenueManagement");
				}
			}

			try
			{
				var venueConfig = config.JsonToObject<Dictionary<DayOfWeek, List<BookingPeriod>>>();

				if (venueConfig != null)
				{
					PostService.SaveVenueConfigs(venue, venueConfig);
				}
			}
			catch (Exception ex)
			{
				LocalLoggingService.Exception(string.Format("/Admin/SaveVenue error: {0}", ex.Message));
			}

			return AlertAndRedirect("保存成功", string.Format("/Admin/VenueGroup/{0}", venueModel.VenueGroupId));
		}

		#endregion

		#region Post

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult PostManagement()
		{
			var groups = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role);

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Post(long id)
		{
			ViewBag.GroupList = UserGroupService.GetGroupsByManager(AdminUser.Id, (RoleLevel)AdminUser.Role);

			var post = id > 0
				? PostService.GetPost(id, false)
				: new PostInfo();

			if (post == null)
			{
				return RedirectToAction("PostManagement", "Admin");
			}

			CommonService.CleanTempFile(CurrentUserId);

			return View(post);
		}

		[HttpPost]
		[ValidateInput(false)]
		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SavePost(PostInfo postModel)
		{
			var post = postModel.Id > 0
				? PostService.GetPost(postModel.Id, false)
				: new PostInfo();

			if (post == null)
			{
				return AlertAndRedirect("保存失败(信息不存在)", "/Admin/PostManagement");
			}

			var group = UserGroupService.GetGroup(postModel.GroupId, false);

			if (group == null)
			{
				return AlertAndRedirect("保存失败(组织信息不存在)", "/Admin/PostManagement");
			}

			post.GroupId = postModel.GroupId;
			post.PostType = postModel.PostType;
			post.Title = postModel.Title;
			post.Content = postModel.Content;
			post.BeginDate = postModel.BeginDate;
			post.EndDate = postModel.EndDate;
			post.Phone = postModel.Phone;
			post.LimitNumber = postModel.LimitNumber;
			post.Fee = postModel.Fee;
			post.Location = postModel.Location;
			post.RelatedPlace = postModel.RelatedPlace;
			post.IsHealthTopic = group.IsHealth;
			post.VenueGroupId = postModel.PostType == (int)PostType.Venue ? postModel.VenueGroupId : 0;
			post.Publisher = AdminUser.Name;
			post.PublishTime = DateTime.Now;
			post.Image = postModel.Image;

			if (post.Id == 0)
			{
				if (!PostService.AddPost(post))
				{
					return AlertAndRedirect("保存失败(添加发布信息失败)", "/Admin/PostManagement");
				}
			}
			else
			{
				if (!PostService.UpdatePost(post))
				{
					return AlertAndRedirect("保存失败(更新发布信息失败)", "/Admin/PostManagement");
				}
			}

			CommonService.SavePostImage(CurrentUserId, post.GroupId, post.Id);

			return AlertAndRedirect("保存成功", "/Admin/PostManagement");
		}
		#endregion

		#region User

		[OutputCache(Duration = 3600)]
		public ActionResult Login()
		{
			return View();
		}

		public ActionResult Header()
		{
			ViewBag.LoginName = AdminUser == null
				? string.Empty : AdminUser.LoginName;

			return PartialView("_Header");
		}

		#endregion

		#region Ajax

		public ActionResult GetGroupManagers(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var managers = UserGroupService.GetManagersByGroup(id)
				.Select(m => new { Id = m.Id, LoginName = m.LoginName, Name = m.Name });

			return Content(managers.ObjectToJson(), "text/json");
		}

		public ActionResult GetGroupsByParent(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var groups = UserGroupService.GetGroupsByParent(id)
				.Select(g => new { Id = g.Id, Name = g.Name });

			return Content(groups.ObjectToJson(), "text/json");
		}

		public ActionResult GetVenuesByGroup(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var venues = PostService.GetVenuesByGroup(id)
				.Select(v => new { Id = v.Id, Name = v.Name });

			return Content(venues.ObjectToJson(), "text/json");
		}

		public ActionResult GetVenueGroupsByGroup(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var venueGroups = PostService.GetVenueGroupsByGroup(id)
				.Select(v => new 
				{ 
					Id = v.Id, 
					Name = v.Name,
					VenueType = PostService.GetVenueTypeName((VenueType)v.VenueType)
				});

			return Content(venueGroups.ObjectToJson(), "text/json");
		}

		public ActionResult GetPostsByGroup(long id, int pageIndex)
		{
			if (AdminUser == null)
			{
				return Content("{\"TotalPage\":0,\"List\":[]}", "text/json");
			}

			var totalNumber = 0;
			var pageSize = 10;
			var posts = PostService.GetPostsByGroup(false, id, false, pageIndex, pageSize, out totalNumber)
				.Select(p => new 
				{ 
					Id = p.Id, 
					Title = p.Title,
					PostType = PostService.GetPostTypeName((PostType)p.PostType),
					BookingNumber = p.BookingNumber,
					Publisher = p.Publisher,
					PublishTime = p.PublishTime.ToString("yyyy-MM-dd HH:mm:ss") 
				});

			var json = string.Format("{{\"TotalPage\":{0},\"List\":{1}}}"
				, totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / pageSize) : 1
				, posts.ObjectToJson());

			return Content(json, "text/json");
		}

		[HttpPost]
		public ActionResult BindManager(string loginName, string name, string password, long groupId)
		{
			var success = false;

			if (AdminUser != null)
			{
				if (!string.IsNullOrWhiteSpace(loginName) && !string.IsNullOrWhiteSpace(name)
					&& !string.IsNullOrWhiteSpace(password) && groupId > 0)
				{
					var user = new UserInfo
					{
						LoginName = loginName,
						Name = name,
						Password = password.ToMd5(),
						Role = (int)RoleLevel.Manager
					};

					if (UserGroupService.AddUser(user))
					{
						success = UserGroupService.AddManagerToGroup(user.Id, groupId);
					}
				}
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemoveManager(long userId, long groupId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = UserGroupService.RemoveManagerFromGroup(userId, groupId);

				if (success)
				{
					UserGroupService.DeleteUser(userId);
				}
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemoveGroup(long groupId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = UserGroupService.DeleteGroup(groupId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemoveVenue(long venueId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.DeleteVenue(venueId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemoveVenueGroup(long venueGroupId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.DeleteVenueGroup(venueGroupId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemovePost(long postId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.DeletePost(postId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult ResetPassword(long userId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = UserGroupService.ResetPassword(userId, "111111");
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		#endregion
	}
}
