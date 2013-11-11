using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

using Hanger.Utility;
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

		[OutputCache(Duration = 3600)]
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

		public ActionResult Login()
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

		public ActionResult SwipeLogin()
		{
			return View();
		}

		[HttpPost]
		public ActionResult SwipeLogin(string userCode)
		{
			var success = UserGroupService.LoginByCode(userCode);

			if (success)
			{
				return RedirectToAction("Index", "Home");
			}
			else
			{
				return AlertAndRedirect("登录失败", "/User/SwipeLogin");
			}
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Register(UserInfo userModel)
		{
			if (string.IsNullOrWhiteSpace(userModel.LoginName) || string.IsNullOrWhiteSpace(userModel.Code))
			{
				return AlertAndRedirect("登录名或卡号为空", "/User/Register");
			}

			var user = new UserInfo
			{
				Code = userModel.Code,
				LoginName = userModel.LoginName,
                Password = StringHelper.GetMd5(userModel.LoginName),
                Name = userModel.Name,
                Gender = userModel.Gender,
                Birth = userModel.Birth,
                Committees = userModel.Committees,
                Mobile = userModel.Mobile,
                Address = userModel.Address,
				Role = (int)RoleLevel.Normal
			};

			var result = UserGroupService.AddUser(user);

			return AlertAndRedirect(EnumHelper.GetDescription(result), "/User/Register");
		}

		[LoginAuthentication(RoleLevel.Normal, "Home", "Index")]
		public ActionResult MyHome(int tag = 0, int pageIndex = 1)
		{
			var totalNumber = 0;

			ViewBag.Tag = tag;
			ViewBag.PageIndex = pageIndex;
			ViewBag.CurrentUserId = CurrentUserId;

			switch (tag)
			{
				case 0:
				case 3:
					var myPosts = tag == 0
						? PostService.GetPostsByGroupsUserConcerned(CurrentUserId, pageIndex, _PostListSize, out totalNumber)
						: PostService.GetPostsByUserConcerned(CurrentUserId, pageIndex, _PostListSize, out totalNumber);

					var myPostsModel = new List<PostModel>();

					foreach (var post in myPosts)
					{
						myPostsModel.Add(new PostModel
						{
							Post = post,
							Group = UserGroupService.GetGroup(post.GroupId, true)
						});
					}

					ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;

					return View("MyPosts", myPostsModel);
				case 1:
					var bookingPosts = PostService.GetPostsByUserBooking(CurrentUserId, pageIndex, _PostListSize, out totalNumber);

					ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;

					return View("MyBooking", bookingPosts);
				case 2:
					var groups = UserGroupService.GetGroupsByUser(CurrentUserId);
					var myGroupsModel = new List<GroupModel>();

					foreach (var group in groups)
					{
						myGroupsModel.Add(new GroupModel
						{
							Group = group,
							IsFollowing = true
						});
					}

					ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;

					return View("MyGroups", myGroupsModel);
				case 4:
					var messages = PostService.GetUserMessages(CurrentUserId, pageIndex, _PostListSize, out totalNumber);
					var myMessagesModel = new List<MessageModel>();

					foreach (var message in messages)
					{
						myMessagesModel.Add(new MessageModel
						{
							Message = message,
							Post = message.PostId > 0 ? PostService.GetPost(message.PostId, true) : null
						});
					}

					ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;

					return View("MyMessages", myMessagesModel);
				default:
					break;
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
