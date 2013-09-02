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
		private static readonly string UploadPhysicalPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\";
		private static readonly string[] AllowImageExtensions = new string[] { ".jpg", ".png", ".jpeg", ".gif" };
		
		public CommonService() { }

		public FileUploadResult UploadTempFile(long userId, string fileType)
		{
			if (userId <= 0)
				return new FileUploadResult(1, "上传失败(登录验证异常)", string.Empty, string.Empty);

			var fileName = string.Empty;
			var extension = string.Empty;

			if (fileType != UploadFileType.Post && fileType != UploadFileType.Logo && fileType != UploadFileType.Avatar)
				return new FileUploadResult(1, "上传失败(文件类型异常)", string.Empty, string.Empty);

			if (HttpContext.Current.Request.Files.Count > 0 && HttpContext.Current.Request.Files[0].ContentLength > 0)
			{
				var uploadFile = HttpContext.Current.Request.Files[0];

				extension = Path.GetExtension(uploadFile.FileName).ToLower();

				if (!AllowImageExtensions.Contains(extension))
				{
					return new FileUploadResult(1, "上传失败(文件格式错误)", string.Empty, string.Empty);
				}

				this.CleanTempFile(userId);

				fileName = string.Format("{0}_temp_{1}{2}", userId.ToString(), fileType, extension);

				FileManager.Upload(uploadFile.InputStream, this.GetTempFilePath(userId), fileName);
			}
			else
			{
				return new FileUploadResult(1, "上传失败(上传文件为空)", string.Empty, string.Empty);
			}

			return new FileUploadResult(0, "上传成功", string.Format("/upload/user/{0}/{1}", userId, fileName), extension);
		}

		public void CleanTempFile(long userId)
		{
			var tempPath = this.GetTempFilePath(userId);

			if (Directory.Exists(tempPath))
			{
				var tempFiles = Directory.GetFiles(tempPath)
					.Where(f => Path.GetFileName(f).StartsWith(userId.ToString() + "_temp_"));

				foreach (var file in tempFiles)
				{
					File.Delete(file);
				}
			}
		}

		public void SaveGroupLogo(long userId, long groupId)
		{
			var realPath = UploadPhysicalPath + string.Format("Group\\{0}\\", groupId);
			var tempFile = this.GetTempFile(userId, UploadFileType.Logo);

			if (tempFile != string.Empty)
			{
				var extension = Path.GetExtension(tempFile);

				if (!Directory.Exists(realPath))
					Directory.CreateDirectory(realPath);

				try
				{
					FileManager.MakeThumbnail(
						tempFile, Path.Combine(realPath, string.Format("{0}{1}", UploadFileType.Logo, extension)), 78, 75, ThumbnailMode.HeightWidth);

					File.Delete(tempFile);
				}
				catch (Exception ex)
				{
					LocalLoggingService.Exception(string.Format("SaveLogo Error:{0}", ex.Message));
				}
			}
		}

		public void SavePostImage(long userId, long groupId, long postId)
		{
			var realPath = UploadPhysicalPath + string.Format("Group\\{0}\\", groupId);
			var tempFile = this.GetTempFile(userId, UploadFileType.Post);

			if (tempFile != string.Empty)
			{
				var extension = Path.GetExtension(tempFile);

				if (!Directory.Exists(realPath))
					Directory.CreateDirectory(realPath);

				try
				{
					File.Move(tempFile, Path.Combine(realPath, string.Format("{0}{1}", postId, extension)));
				}
				catch (Exception ex)
				{
					LocalLoggingService.Exception(string.Format("SavePostImage Error:{0}", ex.Message));
				}
			}
		}

		public void SaveUserAvatar(long userId)
		{
			var realPath = UploadPhysicalPath + string.Format("User\\{0}\\", userId);
			var tempFile = this.GetTempFile(userId, UploadFileType.Avatar);

			if (tempFile != string.Empty)
			{
				var extension = Path.GetExtension(tempFile);

				if (!Directory.Exists(realPath))
					Directory.CreateDirectory(realPath);

				try
				{
					FileManager.MakeThumbnail(
						tempFile, Path.Combine(realPath, string.Format("{0}{1}", UploadFileType.Avatar, extension)), 140, 132, ThumbnailMode.HeightWidth);

					File.Delete(tempFile);
				}
				catch (Exception ex)
				{
					LocalLoggingService.Exception(string.Format("SavePostImage Error:{0}", ex.Message));
				}
			}
		}

		#region private

		private string GetTempFilePath(long userId)
		{
			return UploadPhysicalPath + string.Format("User\\{0}\\", userId);
		}

		private string GetTempFile(long userId, string fileType)
		{
			var tempPath = this.GetTempFilePath(userId);

			if (Directory.Exists(tempPath))
			{
				var tempFiles = Directory.GetFiles(tempPath, string.Format("{0}_temp_{1}.*", userId, fileType));

				foreach (var file in tempFiles)
				{
					return file;
				}
			}

			return string.Empty;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
