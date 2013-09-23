using System;

namespace Wlq.Domain
{
	public class UserMessageInfo : Entity
	{
		public long UserId { get; set; }
		public long MessageId { get; set; }
		public bool IsRead { get; set; }
	}
}
