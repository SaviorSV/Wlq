using System;

namespace Wlq.Domain
{
	public class PostInfo : Entity
	{
		public long GroupId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Image { get; set; }
		public int PostType { get; set; }
		public DateTime BeginDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Location { get; set; }
		public string RelatedPlace { get; set; }
		public string Phone { get; set; }
		public decimal Fee { get; set; }
		public string Remark { get; set; }
		public bool IsHealthTopic { get; set; }
		public long VenueId { get; set; }
		public int LimitNumber { get; set; }
		public int BookingNumber { get; set; }
		public string Publisher { get; set; }
		public DateTime PublishTime { get; set; }

		public PostInfo()
		{
			Title = string.Empty;
			Content = string.Empty;
			Image = string.Empty;
			BeginDate = DateTime.Now.Date;
			EndDate = DateTime.Now.Date.AddMonths(1);
			Location = string.Empty;
			RelatedPlace = string.Empty;
			Phone = string.Empty;
			Remark = string.Empty;
			Publisher = string.Empty;
			PublishTime = DateTime.Now;
		}
	}
}
