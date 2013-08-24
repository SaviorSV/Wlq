using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Wlq.Service;
using System.Web.Security;
using Hanger.Common;
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

		public ActionResult Index()
        {
			if (AdminUser == null)
			{
				return RedirectToAction("Login", "Admin");
			}

            return View();
        }

		public ActionResult AdminManagement()
		{
			if (AdminUser == null)
			{
				return RedirectToAction("Login", "Admin");
			}

			var model = new AdminManagementModel();

			if (AdminUser.Role == (int)RoleLevel.SuperAdmin)
			{
				ViewBag.SuperAdmin = true;

				model.Departments = UserGroupService.GetGroupsByParent(0);
			}
			else
			{
				ViewBag.SuperAdmin = false;

				model.Departments = UserGroupService.GetGroupsByManager(AdminUser.Id)
					.Where(g => g.ParentGroupId == 0);

				if (model.Departments != null && model.Departments.Count() > 0)
				{
					model.Circles = UserGroupService.GetGroupsByParent(model.Departments.First().Id);
				}
			}

			return View(model);
		}

		public ActionResult GroupManagement()
		{
			if (AdminUser == null)
			{
				return RedirectToAction("Login", "Admin");
			}

			IEnumerable<GroupInfo> groups = null;

			if (AdminUser.Role == (int)RoleLevel.SuperAdmin)
			{
				groups = UserGroupService.GetGroupsByParent(0);
			}
			else
			{
				groups = UserGroupService.GetGroupsByManager(AdminUser.Id)
					.Where(g => g.ParentGroupId == 0);
			}

			return View(groups);
		}

		public ActionResult Group(long groupId, long parentGroupId)
		{
			if (AdminUser == null)
			{
				return RedirectToAction("Login", "Admin");
			}

			if (AdminUser.Role != (int)RoleLevel.SuperAdmin
				&& !UserGroupService.IsManagerInGroup(AdminUser.Id, parentGroupId))
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			var parentGroup = UserGroupService.GetGroup(parentGroupId);

			if (parentGroup == null)
			{
				return RedirectToAction("GroupManagement", "Admin");
			}

			ViewBag.ParentGroupName = parentGroup.Name;

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

			return View(new GroupInfo());
		}

		#region Login

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string loginName, string password)
		{
			var success = UserGroupService.Login(loginName, password, true);

			if (success)
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				return AlertAndRedirect("用户名或密码错误", "/Admin/Login");
			}
		}

		public ActionResult SignOut()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Login", "Admin");
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

		[HttpPost]
		public ActionResult BindManager(string loginName, string name, string password, long groupId)
		{
			var success = false;

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

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult RemoveManager(long userId, long groupId)
		{
			var success = UserGroupService.RemoveManagerFromGroup(userId, groupId);

			if (success)
			{
				UserGroupService.DeleteUser(userId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		#endregion
	}
}
