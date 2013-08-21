
namespace Wlq.Domain
{
	public class UserInfo : Entity
	{
		public string LoginName { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string Gender { get; set; }
		public string Birth { get; set; }
		public string Mobile { get; set; }
		public string Committees { get; set; }
		public string Address { get; set; }
		public int Tags { get; set; }
		public int Role { get; set; }

		public UserInfo()
		{
			LoginName = string.Empty;
			Password = string.Empty;
			Name = string.Empty;
			Gender = string.Empty;
			Birth = string.Empty;
			Mobile = string.Empty;
			Committees = string.Empty;
			Address = string.Empty;
		}
	}
}
