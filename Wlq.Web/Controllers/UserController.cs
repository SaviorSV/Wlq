using System.Web.Mvc;
using System.Web.Security;
using Wlq.Web.Fliters;
using Wlq.Domain;

namespace Wlq.Web.Controllers
{
	public class UserController : BaseController
	{
		public ActionResult Header()
		{
			var isLogin = CurrentUser != null;

			ViewBag.IsLogin = isLogin;

			if (isLogin)
				ViewBag.Name = CurrentUser.Name;
			else
				ViewBag.Name = string.Empty;

			return PartialView("_Header");
		}

		[LoginAuthentication(RoleLevel.Normal, "Home", "INdex")]
		public ActionResult Info()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string loginName, string password)
		{
			var success = UserGroupService.Login(loginName, password, false);

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

		[HttpPost]
		public ActionResult LoginForAdmin(string loginName, string password)
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

		public ActionResult SignOutForAdmin()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Login", "Admin");
		}
	}
}
