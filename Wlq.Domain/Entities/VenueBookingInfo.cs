using System;

namespace Wlq.Domain
{
	public class VenueBookingInfo : Entity
	{
		public long VenueId { get; set; }
		public long VenueConfigId { get; set; }
		public long UserId { get; set; }
		public long PostId { get; set; }
		public DateTime BookingDate { get; set; }
		
		public VenueBookingInfo()
		{
			
		}
	}
}
