﻿using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IPostService : IDisposable
	{
		#region Venue

		IEnumerable<VenueInfo> GetVenuesByGroup(long groupId);
		VenueInfo GetVenue(long venueId);
		bool AddVenue(VenueInfo venue);
		bool UpdateVenue(VenueInfo venue);
		bool DeleteVenue(long venueId);

		bool SaveVenueConfigs(long venueId, BookingConfig configs);
		BookingConfig GetVenueConfigs(long venueId);

		#endregion
	}
}
