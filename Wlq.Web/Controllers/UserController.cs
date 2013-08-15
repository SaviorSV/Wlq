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
		public ActionResult Login(string username, string password)
		{
			var user = UserService.GetUser(username, password);

			if (user != null)
			{
				Login(user);

				return RedirectToAction("Index", "Home");
			}
			else
			{
				return AlertAndRedirect("用户名或密码错误", "/");
			}
		}

		private void Login(UserInfo user)
		{
			var ticket = new FormsAuthenticationTicket(
				1,
				user.Id.ToString(),
				DateTime.Now,
				DateTime.Now.AddMinutes(30),
				true,
				string.Empty,
				FormsAuthentication.FormsCookiePath);

			var encTicket = FormsAuthentication.Encrypt(ticket);

			Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
		}

		public ActionResult SignOut()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Index", "Home");
		}

	}
}
