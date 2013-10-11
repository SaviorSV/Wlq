using System;

namespace Wlq.Domain
{
	public class BookingInfo : EntityBase
	{
		public long UserId { get; set; }
		public long PostId { get; set; }
		public string Name { get; set; }
		public string Mobile { get; set; }
		public long VenueId { get; set; }
		public long VenueConfigId { get; set; }
		public int BookingType { get; set; }
		public int InvolvedType { get; set; }
		public DateTime BookingDate { get; set; }
		public bool IsPresent { get; set; }
		
		public BookingInfo()
		{
			Name = string.Empty;
			Mobile = string.Empty;
			BookingDate = DateTime.Now;
		}
	}
}
