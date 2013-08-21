using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wlq.Web.Controllers
{
    public class CommonController : BaseController
    {
		public ActionResult Header()
		{
			var isLogin = CurrentUser != null;

			ViewBag.IsLogin = isLogin;

			if (isLogin)
				ViewBag.LoginName = CurrentUser.Name;
			else
				ViewBag.LoginName = string.Empty;

			return PartialView("_Header");
		}

		public ActionResult Redirect(string message, string url)
		{
			ViewBag.Message = HttpUtility.UrlDecode(message);
			ViewBag.Url = url;

			return PartialView("_Redirect");
		}
    }
}
