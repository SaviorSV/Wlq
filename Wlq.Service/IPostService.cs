using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IPostService : IDisposable
	{
		#region Booking

		bool SaveVenueConfigs(long venueId, BookingConfig configs);
		BookingConfig GetVenueConfigs(long venueId);

		#endregion
	}
}
