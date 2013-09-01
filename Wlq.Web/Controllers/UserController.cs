using System.Web.Mvc;
using System.Web.Security;
using Wlq.Web.Fliters;
using Wlq.Domain;

namespace Wlq.Web.Controllers
{
	public class UserController : BaseController
	{
		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult Info()
		{
			return View(CurrentUser);
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult UpdateUser(UserInfo userModel)
		{
			//todo: user avatar
			CurrentUser.Name = userModel.Name;
			CurrentUser.Gender = userModel.Gender;
			CurrentUser.Birth = userModel.Birth;
			CurrentUser.Committees = userModel.Committees;
			CurrentUser.Mobile = userModel.Mobile;
			CurrentUser.Address = userModel.Address;
			CurrentUser.Tags = userModel.Tags;

			var message = UserGroupService.UpdateUser(CurrentUser)
				? "保存成功" : "保存失败";

			return AlertAndRedirect(message, "/User/Info");
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
