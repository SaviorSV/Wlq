
namespace Wlq.Web.Models
{
	public class ChangePasswordModel
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string NewPasswordVerify { get; set; }
	}
}