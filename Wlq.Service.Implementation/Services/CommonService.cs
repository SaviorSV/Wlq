using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Persistence;
using Wlq.Service;
using Wlq.Service.Utility;

namespace Wlq.Service.Implementation
{
	public class CommonService : Disposable, ICommonService
	{
		public CommonService() { }

		public FileUploadResult UploadTempFile(long userId, string fileType)
		{
			if (userId <= 0)
				return new FileUploadResult(1, "上传失败(登录验证异常)", string.Empty, string.Empty);

			var fileName = string.Empty;
			var extension = string.Empty;

			if (fileType != UploadFileType.Post && fileType != UploadFileType.Logo)
				return new FileUploadResult(1, "上传失败(文件类型异常)", string.Empty, string.Empty);

			if (HttpContext.Current.Request.Files.Count > 0 && HttpContext.Current.Request.Files[0].ContentLength > 0)
			{
				var uploadFile = HttpContext.Current.Request.Files[0];

				extension = Path.GetExtension(uploadFile.FileName).ToLower();

				if (!FileManager.AllowImageExtensions.Contains(extension))
				{
					return new FileUploadResult(1, "上传失败(文件格式错误)", string.Empty, string.Empty);
				}

				fileName = string.Format("{0}_{1}{2}", userId.ToString(), fileType, extension);

				FileManager.Upload(uploadFile.InputStream, FileManager.TempFilePhysicalPath, fileName);
			}
			else
			{
				return new FileUploadResult(1, "上传失败(上传文件为空)", string.Empty, string.Empty);
			}

			return new FileUploadResult(0, "上传成功", string.Format("/upload/temp/{0}", fileName), extension);
		}

		public void CleanTempFile(long userId)
		{
			if (Directory.Exists(FileManager.TempFilePhysicalPath))
			{
				var tempFiles = Directory.GetFiles(FileManager.TempFilePhysicalPath)
					.Where(f => Path.GetFileName(f).StartsWith(userId.ToString() + "_"));

				foreach (var file in tempFiles)
				{
					File.Delete(file);
				}
			}
		}

		public void SaveLogo(long userId, long groupId)
		{
			var realPath = FileManager.RealFilePhysicalPath + string.Format("{0}\\", groupId);

			if (Directory.Exists(FileManager.TempFilePhysicalPath))
			{
				var tempFiles = Directory.GetFiles(FileManager.TempFilePhysicalPath, string.Format("{0}_{1}.*", userId, UploadFileType.Logo));

				foreach (var file in tempFiles)
				{
					var extension = Path.GetExtension(file);

					if (!Directory.Exists(realPath))
						Directory.CreateDirectory(realPath);

					try
					{
						FileManager.MakeThumbnail(
							file, Path.Combine(realPath, string.Format("{0}{1}", UploadFileType.Logo, extension)), 78, 75, ThumbnailMode.HeightWidth);

						File.Delete(file);
					}
					catch (Exception ex)
					{
						LocalLoggingService.Exception(string.Format("SaveLogo Error:{0}", ex.Message));
					}

					break;
				}
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
