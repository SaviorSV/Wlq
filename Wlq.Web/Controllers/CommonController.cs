using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Service.Utility;
using Wlq.Web.Models;

namespace Wlq.Web.Controllers
{
    public class CommonController : BaseController
    {
		public ActionResult Redirect(string message, string url)
		{
			ViewBag.Message = HttpUtility.UrlDecode(message);
			ViewBag.Url = url;

			return PartialView("_Redirect");
		}

		[HttpPost]
		public ActionResult Upload(string type)
		{
			var result = CommonService.UploadTempFile(CurrentUserId, type);

			return Content(result.ObjectToJson());
		}


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

		[OutputCache(Duration = 3600)]
		public ActionResult LeftMenu()
		{
			var model = new LeftMenuModel();
			var totalNumber = 0;

			model.Departments = UserGroupService.GetGroupsByParent(0);
			model.Circles = UserGroupService.GetGroups(
				g => g.ParentGroupId > 0, 1, 9, out totalNumber);

			return PartialView("_LeftMenu", model);
		}
    }
}
