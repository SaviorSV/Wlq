using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Wlq.Service;
using System.Web.Security;
using Hanger.Common;

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

		public ActionResult GetGroupManagers(long groupId)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var managers = UserGroupService.GetManagersByGroup(groupId)
				.Select(m => new { Id = m.Id, LoginName = m.LoginName, Name = m.Name });

			return Content(managers.ObjectToJson(), "text/json");
		}

		public ActionResult GetGroupsByParent(long parentId)
		{
			if (AdminUser == null)
			{
				return Content("[]", "text/json");
			}

			var groups = UserGroupService.GetGroupsByParent(parentId)
				.Select(g => new { Id = g.Id, Name = g.Name });

			return Content(groups.ObjectToJson(), "text/json");
		}

		#endregion
	}
}
