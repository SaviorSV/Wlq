﻿
namespace Wlq.Domain
{
	public class UserInfo : Entity
	{
		public string LoginName { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string Gender { get; set; }
		public string Mobile { get; set; }
		public string Address { get; set; }
		public bool IsSystem { get; set; }
	}
}
