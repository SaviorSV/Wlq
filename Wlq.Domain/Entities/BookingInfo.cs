using System;

namespace Wlq.Domain
{
	public class BookingInfo : Entity
	{
		public long UserId { get; set; }
		public long PostId { get; set; }
		public string Name { get; set; }
		public string Mobile { get; set; }
		public long VenueConfigId { get; set; }
		public DateTime BookingDate { get; set; }
		
		public BookingInfo()
		{
			Name = string.Empty;
			Mobile = string.Empty;
			BookingDate = DateTime.Now;
		}
	}
}
