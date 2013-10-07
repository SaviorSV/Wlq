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
		public decimal Fee { get; set; }
		public string Remark { get; set; }
		public bool IsHealthTopic { get; set; }
		public long VenueGroupId { get; set; }
		public int LimitNumber { get; set; }
		public int BookingNumber { get; set; }
		public int BookingTypes { get; set; }
		public int InvolvedTypes { get; set; }
		public string PhoneBookingNumber { get; set; }
		public string PhoneBookingTime { get; set; }
		public string SpotBookingNumber { get; set; }
		public string SpotBookingAddress { get; set; }
		public string SpotBookingTime { get; set; }
		public string Publisher { get; set; }
		public DateTime PublishTime { get; set; }

		public PostInfo()
		{
			Title = string.Empty;
			Content = string.Empty;
			Image = string.Empty;
			BeginDate = DateTime.Now.Date;
			EndDate = DateTime.Now.Date.AddMonths(1);
			Remark = string.Empty;
			Publisher = string.Empty;
			PublishTime = DateTime.Now;
		}
	}
}
