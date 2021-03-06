﻿
namespace Wlq.Domain
{
	public class GroupInfo : EntityBase
	{
		public string Name { get; set; }
		public string Logo { get; set; }
		public long ParentGroupId { get; set; }
		public int GroupType { get; set; }
		public string Address { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string WorkTime { get; set; }
		public string Introduction { get; set; }
		public bool IsHealth { get; set; }

		public GroupInfo()
		{
			Name = string.Empty;
			Logo = string.Empty;
			Address = string.Empty;
			Phone = string.Empty;
			Email = string.Empty;
			WorkTime = string.Empty;
			Introduction = string.Empty;
		}
	}
}
