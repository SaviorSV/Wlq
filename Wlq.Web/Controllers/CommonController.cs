using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Service.Utility;

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

    }
}
