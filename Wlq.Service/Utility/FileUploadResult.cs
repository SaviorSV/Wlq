
namespace Wlq.Service.Utility
{
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
}
