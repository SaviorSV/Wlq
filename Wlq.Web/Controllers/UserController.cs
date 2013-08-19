using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Wlq.Domain;


namespace Wlq.Web.Controllers
{
	public class UserController : BaseController
	{
		[HttpPost]
		public ActionResult Login(string loginName, string password)
		{
			var success = UserGroupService.Login(loginName, password);

			if (success)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return AlertAndRedirect("用户名或密码错误", "/");
			}
		}

		public ActionResult SignOut()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Index", "Home");
		}

	}
}
