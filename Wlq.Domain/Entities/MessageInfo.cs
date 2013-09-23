using System;

namespace Wlq.Domain
{
	public class MessageInfo : Entity
	{
		public string Title { get; set; }
		public string Content { get; set; }
		public long SenderId { get; set; }
		public DateTime SendTime { get; set; }

		public MessageInfo()
		{
			Title = string.Empty;
			Content = string.Empty;
			SendTime = DateTime.Now;
		}
	}
}
