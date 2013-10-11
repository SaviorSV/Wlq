using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class AdminPostBookerModel
	{
		public BookingInfo Booking { get; set; }
		public VenueConfigInfo VenueConfig { get; set; }
	}
}