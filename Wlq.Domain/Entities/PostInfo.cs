using System;

namespace Wlq.Domain
{
	public class PostInfo : Entity
	{
		public string Title { get; set; }
		public string Content { get; set; }
		public string Image { get; set; }
		public long GroupId { get; set; }
		public string Publisher { get; set; }
		public int PostType { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public long VenueId { get; set; }
		public int BookingNumber { get; set; }

		public PostInfo()
		{
			Title = string.Empty;
			Content = string.Empty;
			Image = string.Empty;
		}
	}
}
