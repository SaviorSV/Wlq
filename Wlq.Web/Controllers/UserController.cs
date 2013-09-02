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
			CommonService.CleanTempFile(CurrentUserId);

			return View(CurrentUser);
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult UpdateUser(UserInfo userModel)
		{
			CurrentUser.Name = userModel.Name;
			CurrentUser.Gender = userModel.Gender;
			CurrentUser.Birth = userModel.Birth;
			CurrentUser.Committees = userModel.Committees;
			CurrentUser.Mobile = userModel.Mobile;
			CurrentUser.Address = userModel.Address;
			CurrentUser.Tags = userModel.Tags;
			CurrentUser.Avatar = userModel.Avatar;

			var success =  UserGroupService.UpdateUser(CurrentUser);
			var message = success ? "保存成功" : "保存失败";

			if (success)
			{
				CommonService.SaveUserAvatar(CurrentUserId);
			}

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
