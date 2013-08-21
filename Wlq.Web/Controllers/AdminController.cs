using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Wlq.Service;

namespace Wlq.Web.Controllers
{
    public class AdminController : BaseController
    {
		private UserInfo _adminUser;

		protected UserInfo AdminUser
		{
			get
			{
				if (_adminUser == null)
				{
					if (CurrentUser != null && CurrentUser.IsAdmin)
					{
						_adminUser = CurrentUser;
					}
				}

				return _adminUser;
			}
		}

        public ActionResult Index()
        {
			if (AdminUser == null)
			{
				return RedirectToAction("Login", "Admin");
			}

            return View();
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

		public ActionResult Header()
		{
			ViewBag.LoginName = AdminUser == null
				? string.Empty : AdminUser.LoginName;

			return PartialView("_Header");
		}

		#endregion

	}
}
