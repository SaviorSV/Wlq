﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Domain;
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
            return View();
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
			GroupInfo parentGroup = null;

			if (!CheckParentGroupIsLegal(parentGroupId, ref parentGroup))
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			ViewBag.ParentGroupName = parentGroup.Name;

			CommonService.CleanTempFile(CurrentUserId);

			if (groupId > 0)
			{
				var group = UserGroupService.GetGroup(groupId);

				if (group != null)
				{
					if (group.ParentGroupId == parentGroupId)
					{
						return View(group);
					}
					else
					{
						return RedirectToAction("GroupManagement", "Admin");
					}
				}
			}

			return View(new GroupInfo { ParentGroupId = parentGroupId });
		}

		[LoginAuthentication(RoleLevel.Manager, "Admin", "Login")]
		public ActionResult SaveGroup(GroupInfo groupModel)
		{
			GroupInfo parentGroup = null;

			if (!CheckParentGroupIsLegal(groupModel.ParentGroupId, ref parentGroup))
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			var group = groupModel.Id > 0
				? UserGroupService.GetGroup(groupModel.Id)
				: new GroupInfo();

			if (group == null)
			{
				return AlertAndRedirect("保存失败(该组不存在)", "/Admin/GroupManagement");
			}

			group.Name = groupModel.Name;
			group.ParentGroupId = groupModel.ParentGroupId;
			group.GroupType = (int)GroupType.Circle;
			group.Logo = groupModel.Logo;

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

			CommonService.SaveLogo(CurrentUserId, group.Id);

			return AlertAndRedirect("保存成功", "/Admin/GroupManagement");
		}

		private bool CheckParentGroupIsLegal(long parentGroupId, ref GroupInfo parentGroup)
		{
			if (AdminUser.Role != (int)RoleLevel.SuperAdmin
				&& !UserGroupService.IsManagerInGroup(AdminUser.Id, parentGroupId))
			{
				return false;
			}

			parentGroup = UserGroupService.GetGroup(parentGroupId);

			if (parentGroup == null)
			{
				return false;
			}

			return true;
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
		public ActionResult Venue(long venueId, long groupId)
		{
			var group = UserGroupService.GetGroup(groupId);

			if (group == null)
			{
				return RedirectToAction("VenueManagement", "Admin");
			}

			var venueConfig = PostService.GetVenueConfigs(venueId);

			ViewBag.GroupName = group.Name;
			ViewBag.VenueConfig = venueConfig.ToJson();

			if (venueId > 0)
			{
				var venue = PostService.GetVenue(venueId);

				if (venue != null)
				{
					if (venue.GroupId == groupId)
					{
						return View(venue);
					}
					else
					{
						return RedirectToAction("VenueManagement", "Admin");
					}
				}
			}

			return View(new VenueInfo { GroupId = groupId });
		}

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

			return AlertAndRedirect("保存成功", "/Admin/VenueManagement");
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
		public ActionResult Post(long postId, long groupId)
		{
			//todo:
			return View();
		}

		#endregion

		#region User

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

		public ActionResult GetPostsByGroup(long id)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var totalNumber = 0;
			var posts = PostService.GetPostsByGroup(id, false, 0, 0, out totalNumber)
				.Select(p => new 
				{ 
					Id = p.Id, 
					Title = p.Title,
					PostType = PostService.GetPostTypeName((PostType)p.PostType),
					Publisher = p.Publisher,
					LastModified = p.LastModified.ToString("yyyy-MM-dd HH:mm:ss") 
				});

			return Content(posts.ObjectToJson(), "text/json");
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
		public ActionResult RemovePost(long postId)
		{
			var success = false;

			if (AdminUser != null)
			{
				success = PostService.DeletePost(postId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		#endregion
	}
}
