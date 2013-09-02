using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using Hanger.Common;

namespace Wlq.Service.Utility
{
	public class FileManager
	{
		public static readonly string[] AllowImageExtensions = new string[] { ".jpg", ".png", ".jpeg", ".gif" };
		public static readonly string UploadPhysicalPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\";

		public static void Upload(Stream stream, string physicalPath, string fileName)
		{
			var bufferSize = 256;
			var bytesRead = 0;

			var buffer = new byte[bufferSize];

			if (!Directory.Exists(physicalPath))
			{
				Directory.CreateDirectory(physicalPath);
			}

			var fileStream = File.Create(Path.Combine(physicalPath, fileName), bufferSize);

			try
			{
				do
				{
					bytesRead = stream.Read(buffer, 0, bufferSize);

					if (bytesRead > 0)
					{
						fileStream.Write(buffer, 0, bytesRead);
					}
				} while (bytesRead > 0);
			}
			catch (Exception ex)
			{
				LocalLoggingService.Exception(ex.ToString());
			}
			finally
			{
				stream.Close();
				fileStream.Close();
			}
		}

		public static void MakeThumbnail(string originalPath, string thumbnailPath, int width, int height, ThumbnailMode mode)
		{
			var originalImage = Image.FromFile(originalPath);

			var towidth = width;
			var toheight = height;

			var ow = originalImage.Width;
			var oh = originalImage.Height;

			int x = 0, y = 0;

			switch (mode)
			{
				case ThumbnailMode.HeightWidth:
					break;
				case ThumbnailMode.Width:
					toheight = originalImage.Height * width / originalImage.Width;
					break;
				case ThumbnailMode.Height:
					towidth = originalImage.Width * height / originalImage.Height;
					break;
				case ThumbnailMode.Cut:
					if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
					{
						oh = originalImage.Height;
						ow = originalImage.Height * towidth / toheight;
						y = 0;
						x = (originalImage.Width - ow) / 2;
					}
					else
					{
						ow = originalImage.Width;
						oh = originalImage.Width * height / towidth;
						x = 0;
						y = (originalImage.Height - oh) / 2;
					}
					break;
				default:
					break;
			}

			var bitmap = new Bitmap(towidth, toheight);

			var graphics = Graphics.FromImage(bitmap);

			graphics.InterpolationMode = InterpolationMode.High;

			graphics.SmoothingMode = SmoothingMode.HighQuality;

			graphics.Clear(Color.Transparent);

			graphics.DrawImage(
				originalImage,
				new Rectangle(0, 0, towidth, toheight),
				new Rectangle(x, y, ow, oh),
				GraphicsUnit.Pixel);

			try
			{
				bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
			}
			catch (Exception ex)
			{
				LocalLoggingService.Exception(ex.ToString());
			}
			finally
			{
				originalImage.Dispose();
				bitmap.Dispose();
				graphics.Dispose();
			}
		}
	}

	public class UploadFileType
	{
		public const string Logo = "logo";
		public const string Post = "post";
		public const string Avatar = "avatar";
		public const string File = "file";
	}

	public class FileUploadResult
	{
		/// <summary>
		/// success:0, error:1
		/// </summary>
		public int Error { get; set; }
		public string Message { get; set; }
		public string Url { get; set; }
		public string Extention { get; set; }

		public FileUploadResult(int error, string message, string url, string extension)
		{
			this.Error = error;
			this.Message = message;
			this.Url = url;
			this.Extention = extension;
		}
	}

	public enum ThumbnailMode
	{
		/// <summary>
		/// 指定高宽缩放（可能变形）
		/// </summary>
		HeightWidth = 1,

		/// <summary>
		/// 指定宽，高按比例  
		/// </summary>
		Width = 2,

		/// <summary>
		/// 指定高，宽按比例
		/// </summary>
		Height = 3,

		/// <summary>
		/// 指定高宽裁减（不变形）
		/// </summary>
		Cut = 4
	}
}
