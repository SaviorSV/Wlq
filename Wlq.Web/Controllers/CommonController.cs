using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wlq.Service.Utility;
using System.IO;

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

		[HttpPost]
		public ActionResult Upload(string type)
		{
			if (CurrentUser == null)
				return UploadResult(false, "上传失败", string.Empty, string.Empty);

			var webUrl = "/upload/temp/";
			var fileName = string.Empty;
			var extension = string.Empty;

			if (type != UploadFileType.Posts && type != UploadFileType.Logo)
				return UploadResult(false, "上传失败", string.Empty, string.Empty);

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
			{
				var allowExtensions = new string[] { };
				var uploadFile = Request.Files[0];

				extension = Path.GetExtension(uploadFile.FileName).ToLower();

				switch (type)
				{
					case UploadFileType.Logo:
					case UploadFileType.Posts:
						allowExtensions = FileManager.AllowImageExtensions;
						break;
					default:
						break;
				}

				if (!allowExtensions.Contains(extension))
				{
					return UploadResult(false, "上传文件格式错误", string.Empty, string.Empty);
				}

				fileName = string.Format("{0}_{1}{2}", CurrentUser.Id.ToString(), type, extension);

				var physicalPath = Request.PhysicalApplicationPath + "\\Upload\\temp\\";

				try
				{
					FileManager.Upload(uploadFile.InputStream, physicalPath, fileName);
				}
				catch
				{
					return UploadResult(false, "上传失败", string.Empty, string.Empty);
				}
			}
			else
			{
				return UploadResult(false, "上传失败", string.Empty, string.Empty);
			}

			webUrl += fileName;

			return UploadResult(true, "上传成功", webUrl, extension);
		}

		private ActionResult UploadResult(bool success, string message, string url, string extention)
		{
			return Content(string.Format("{{'error':'{0}','message':'{1}','url':'{2}','extention':'{3}'}}"
				, success ? 0 : 1, message, url, extention));
		}
    }
}
