using System.Web.Mvc;
using System.Web.Security;

using Wlq.Domain;
using Wlq.Service;
using Wlq.Web.Fliters;
using Wlq.Web.Models;

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

			var success = UserGroupService.UpdateUser(CurrentUser);
			var message = success ? "保存成功" : "保存失败";

			if (success)
			{
				CommonService.SaveUserAvatar(CurrentUserId);
			}

			return AlertAndRedirect(message, "/User/Info");
		}

		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			var message = string.Empty;

			if (model.NewPassword != model.NewPasswordVerify)
			{
				message = "两次密码输入不一致";
			}
			else
			{
				var result = UserGroupService.ChangePassword(CurrentUser, model.OldPassword, model.NewPassword);

				switch (result)
				{
					case ChangePasswordResult.Success:
						message = "修改成功";
						break;
					case ChangePasswordResult.OldPasswordWrong:
						message = "原密码不正确";
						break;
					case ChangePasswordResult.Error:
						message = "修改失败";
						break;
					default:
						break;
				}
			}

			return AlertAndRedirect(message, "/User/ChangePassword");
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

		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult MyHome()
		{
			return View();
		}
	}
}
