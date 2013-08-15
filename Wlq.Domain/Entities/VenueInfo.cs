﻿
namespace Wlq.Domain
{
	public class VenueInfo : Entity
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public long GroupId { get; set; }
		public string BookingConfig { get; set; }

		public VenueInfo()
		{
			Name = string.Empty;
			Address = string.Empty;
			BookingConfig = new BookingTimeConfig().ToJson();
		}
	}
}
