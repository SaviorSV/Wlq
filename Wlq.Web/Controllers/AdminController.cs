using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hanger.Common;
using Hanger.Utility;
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

			model.Departments = UserGroupService.GetGroupsByManager(AdminUser)
				.Where(g => g.GroupType == (int)GroupType.Department);

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
			var groups = UserGroupService.GetGroupsByManager(AdminUser)
				.Where(g => g.GroupType == (int)GroupType.Department);

			ViewBag.IsSuperAdmin = AdminUser.Role == (int)RoleLevel.SuperAdmin;

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Group(long groupId, long parentGroupId)
		{
			var group = groupId > 0
				? UserGroupService.GetGroup(groupId, false)
				: new GroupInfo { ParentGroupId = parentGroupId };

			var parentGroup = UserGroupService.GetGroup(parentGroupId, false);

			ViewBag.ParentGroupName = parentGroup == null ? "无" : parentGroup.Name;


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
			if (groupModel.Id == 0 && groupModel.ParentGroupId == 0 && CurrentUser.Role != (int)RoleLevel.SuperAdmin)
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
			group.IsHealth = groupModel.IsHealth;

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
			var groups = UserGroupService.GetGroupsByManager(AdminUser);

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
			model.Groups = UserGroupService.GetGroupsByManager(AdminUser);

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
				return AlertAndRedirect("保存失败", "/Admin/VenueManagement");
			}

			venueGroup.Name = venueGroupModel.Name;
			venueGroup.VenueType = venueGroupModel.VenueType;
			venueGroup.GroupId = venueGroupModel.GroupId;
			venueGroup.Phone = venueGroupModel.Phone;
			venueGroup.Address = venueGroupModel.Address;
			venueGroup.PostType = venueGroupModel.PostType;

			if (venueGroup.Id == 0)
			{
				if (!PostService.AddVenueGroup(venueGroup))
				{
					return AlertAndRedirect("保存失败(添加失败)", "/Admin/VenueManagement");
				}
			}
			else
			{
				if (!PostService.UpdateVenueGroup(venueGroup))
				{
					return AlertAndRedirect("保存失败(更新失败)", "/Admin/VenueManagement");
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
			venue.Phone = venueModel.Phone;
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
			var groups = UserGroupService.GetGroupsByManager(AdminUser);

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult UnAuditedPost()
		{
			var groups = UserGroupService.GetGroupsByManager(AdminUser);

			return View(groups);
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult Post(long id)
		{
			ViewBag.GroupList = UserGroupService.GetGroupTreeByManager(AdminUser);

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
			
			post.LimitNumber = postModel.LimitNumber;
			post.Fee = postModel.Fee;
			post.Remark = postModel.Remark;
			post.IsHealthTopic = group.IsHealth;
			post.VenueGroupId = postModel.VenueGroupId;
			post.Publisher = !string.IsNullOrEmpty(post.Publisher) ? post.Publisher : AdminUser.Name;
			post.PublishTime = DateTime.Now;
			post.Image = postModel.Image;
			post.BookingTypes = postModel.BookingTypes;
			post.InvolvedTypes = postModel.InvolvedTypes;
			post.PhoneBookingNumber = postModel.PhoneBookingNumber;
			post.PhoneBookingTime = postModel.PhoneBookingTime;
			post.SpotBookingNumber = postModel.SpotBookingNumber;
			post.SpotBookingTime = postModel.SpotBookingTime;
			post.SpotBookingAddress = postModel.SpotBookingAddress;
			post.Phone = postModel.Phone;
			post.Address = postModel.Address;
			post.IsAudited = group.GroupType == (int)GroupType.Department ? true : false;
			post.IsLongterm = postModel.IsLongterm;

			if (postModel.IsLongterm)
			{
				post.BeginDate = new DateTime(2000, 1, 1);
				post.EndDate = new DateTime(2099, 1, 1);
			}
			else
			{
				post.BeginDate = postModel.BeginDate;
				post.EndDate = postModel.EndDate;
			}

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

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SigninBooking()
		{
			return View();
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult PostBookers(long id, int pageIndex = 1)
		{
			var pageSize = 20;
			var totalNumber = 0;
			var bookingList = PostService.GetBookingList(id, pageIndex, pageSize, out totalNumber);

			ViewBag.PostId = id;
			ViewBag.PageIndex = pageIndex;
			ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / pageSize) : 1;

			var model = new List<AdminPostBookerModel>();

			foreach (var booking in bookingList)
			{
				model.Add(new AdminPostBookerModel
				{
					Booking = booking,
					Venue = booking.VenueId > 0 ? PostService.GetVenue(booking.VenueId) : null,
					VenueConfig = booking.VenueConfigId > 0 ? PostService.GetVenueConfig(booking.VenueConfigId) : null
				});
			}

			return View(model);
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

		public ActionResult GetBookingList(string userCode)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var bookingList = PostService.GetBookingListByUserCode(userCode, AdminUser)
				.Select(b => new
				{ 
					Id = b.Id,
					PostTitle = GetPostTile(b.PostId),
					Name = b.Name,
					Mobile = b.Mobile,
					VenueName = GetVenueName(b.VenueId),
					DateShow = GetBookingDateShow(b),
					IsPresent = b.IsPresent,
					PresentTime = b.PresentTime.ToString("yyyy-MM-dd HH:mm:ss")
				});

			return Content(bookingList.ObjectToJson(), "text/json");
		}

		public ActionResult GetGroupManagers(long id, string keyword)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var managers = UserGroupService.GetManagersByGroupTree(id, AdminUser, keyword)
				.Select(m => new
				{
					Id = m.Id,
					LoginName = m.LoginName,
					Name = m.Name,
					Group = this.GetGroupByManager(m)
				});

			return Content(managers.ObjectToJson(), "text/json");
		}

		public ActionResult GetGroupsByParent(long id, string keyword)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var groups = UserGroupService.GetGroupTreeByParent(id, AdminUser, keyword)
				.Select(g => new
				{
					Id = g.Id,
					Name = g.Name,
					ParentGroupId = g.ParentGroupId,
					ParentGroupName = g.ParentGroupId > 0 ? this.GetGroupName(g.ParentGroupId) : "-",
				});

			return Content(groups.ObjectToJson(), "text/json");
		}

		public ActionResult GetVenueGroupsByGroup(long id, int postType = (int)PostType.All)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var venueGroups = PostService.GetVenueGroupsByGroup(id, postType)
				.Select(v => new
				{
					Id = v.Id,
					Name = v.Name,
					Phone = v.Phone,
					Address = v.Address,
					PostTypeName = v.PostType == (int)PostType.Venue
						? "场馆" : EnumHelper.GetDescription((PostType)v.PostType)
				});

			return Content(venueGroups.ObjectToJson(), "text/json");
		}

		public ActionResult GetVenueConfigs(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var venueConfigs = PostService.GetVenueConfigs(id);
			var configs = venueConfigs[DateTime.Today.DayOfWeek]
				.Select(c => new
				{
					Id = c.VenueConfigId,
					DayOfWeek = DateTime.Today.DayOfWeek.ToString(),
					BeginTime = string.Format("{0}:{1}", (c.BeginTime / 100).ToString().PadLeft(2, '0'), (c.BeginTime % 100).ToString().PadLeft(2, '0')),
					EndTime = string.Format("{0}:{1}", (c.EndTime / 100).ToString().PadLeft(2, '0'), (c.EndTime % 100).ToString().PadLeft(2, '0'))
				});

			return Content(configs.ObjectToJson(), "text/json");
		}

		public ActionResult GetPostsByGroup(long id, int pageIndex, string keyword)
		{
			if (AdminUser == null)
			{
				return Content("{\"TotalPage\":0,\"List\":[]}", "text/json");
			}

			var totalNumber = 0;
			var pageSize = 10;
			var posts = PostService.GetPostsByGroupTree(id, AdminUser, keyword, pageIndex, pageSize, out totalNumber)
				.Select(p => new
				{
					Id = p.Id,
					GroupName = this.GetGroupName(p.GroupId),
					Title = p.Title,
					PostType = EnumHelper.GetDescription((PostType)p.PostType),
					BookingNumber = p.BookingNumber,
					Publisher = p.Publisher,
					PublishTime = p.PublishTime.ToString("yyyy-MM-dd HH:mm:ss"),
					Status = p.IsAudited ? "已审核" : "未审核"
				});

			var json = string.Format("{{\"TotalPage\":{0},\"List\":{1}}}"
				, totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / pageSize) : 1
				, posts.ObjectToJson());

			return Content(json, "text/json");
		}

		public ActionResult GetUnAuditedPostsByGroup(long id, int pageIndex, string keyword)
		{
			if (AdminUser == null)
			{
				return Content("{\"TotalPage\":0,\"List\":[]}", "text/json");
			}

			var totalNumber = 0;
			var pageSize = 10;
			var posts = PostService.GetPostsByGroupTreeUnAudited(id, AdminUser, keyword, pageIndex, pageSize, out totalNumber)
				.Select(p => new
				{
					Id = p.Id,
					GroupName = this.GetGroupName(p.GroupId),
					Title = p.Title,
					PostType = EnumHelper.GetDescription((PostType)p.PostType),
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
						Password = StringHelper.GetMd5(password),
						Role = (int)RoleLevel.Manager
					};

					if (UserGroupService.AddUser(user) == AddUserResult.Success)
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
		public ActionResult SuspendVenue(long venueId, bool suspend)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.SuspendVenue(venueId, suspend);
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
		public ActionResult PassPost(long postId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.AuditPost(postId);
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

		[HttpPost]
		public ActionResult SendMessageToPostBookers(long postId, string content)
		{
			var success = false;

			content = HttpUtility.UrlDecode(content);

			if (AdminUser != null && !string.IsNullOrWhiteSpace(content))
			{
				success = PostService.SendMessageToPostBookers(postId, string.Empty, content, AdminUser.Id);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult SendMessageToGroupMembers(long groupId, string content)
		{
			var success = false;

			content = HttpUtility.UrlDecode(content);

			if (AdminUser != null && !string.IsNullOrWhiteSpace(content))
			{
				success = PostService.SendMessageToGroupMembers(groupId, string.Empty, content, AdminUser.Id);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult SigninForBooking(long id)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.SigninForBooking(id);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		private string GetGroupName(long groupId)
		{
			var group = UserGroupService.GetGroup(groupId, true);

			return group != null ? group.Name : "-";
		}

		//todo: many to many
		private dynamic GetGroupByManager(UserInfo manager)
		{
			var group = UserGroupService.GetGroupsByManager(manager).FirstOrDefault();

			if (group != null)
			{
				return new { Name = group.Name, Level = GetGroupLevel((GroupType)group.GroupType) };
			}

			return new { Name = "-", Level = "-" };
		}

		private string GetGroupLevel(GroupType type)
		{
			switch (type)
			{
				case GroupType.Department:
					return "部门管理员";
				case GroupType.Circle:
					return "二级组织管理员";
				default:
					break;
			}

			return "-";
		}

		private string GetVenueName(long venueId)
		{
			var venue = venueId > 0 ? PostService.GetVenue(venueId) : null;

			return venue != null ? venue.Name : "-";
		}

		private string GetPostTile(long postId)
		{
			var post = PostService.GetPost(postId, true);

			return post != null ? post.Title : "-";
		}

		private string GetBookingDateShow(BookingInfo booking)
		{
			var dateShow = booking.BookingDate.ToString("yyyy-MM-dd");
			var venueConfig = booking.VenueConfigId > 0 ? PostService.GetVenueConfig(booking.VenueConfigId) : null;

			if (venueConfig != null)
			{
				dateShow += string.Format("({0}:00-{1}:00)", venueConfig.BegenTime / 100, venueConfig.EndTime / 100);
			}

			return dateShow;
		}

		#endregion
	}
}
