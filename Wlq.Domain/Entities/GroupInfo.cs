﻿
namespace Wlq.Domain
{
	public class GroupInfo : Entity
	{
		public string Name { get; set; }
		public string Logo { get; set; }
		public long ParentGroupId { get; set; }
		public int GroupType { get; set; }

		public GroupInfo()
		{
			Name = string.Empty;
			Logo = string.Empty;
		}
	}
}
