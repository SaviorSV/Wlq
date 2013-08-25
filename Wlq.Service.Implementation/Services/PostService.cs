using System;
using System.Linq;

using Hanger.Common;
using Wlq.Service;
using Wlq.Persistence;
using Wlq.Domain;
using System.Collections.Generic;

namespace Wlq.Service.Implementation
{
	public class PostService : Disposable, IPostService
	{
		private readonly DatabaseContext _databaseContext;

		public PostService(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		#region booking

		public IEnumerable<VenueInfo> GetVenuesByGroup(long groupId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			return venueRepository.GetAll()
				.Where(v => v.GroupId == groupId);
		}

		public VenueInfo GetVenue(long venueId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			return venueRepository.GetById(venueId);
		}

		public bool AddVenue(VenueInfo venue)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			venueRepository.Add(venue);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool UpdateVenue(VenueInfo venue)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			venueRepository.Update(venue);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool DeleteVenue(long venueId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			venueRepository.DeleteById(venueId);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool SaveVenueConfigs(long venueId, BookingConfig configs)
		{
			//todo: SaveVenueConfigs
			throw new NotImplementedException();
		}

		public BookingConfig GetVenueConfigs(long venueId)
		{
			var configs = _databaseContext.VenueConfigs.Where(v => v.VenueId == venueId);
			var bookingConfig = new BookingConfig();

			foreach (var config in configs)
			{
				try
				{
					bookingConfig.AddPeriod((DayOfWeek)config.DaysOfWeek, config.BegenTime, config.EndTime);
				}
				catch (Exception ex)
				{
					LocalLoggingService.Exception(ex.Message);
					continue;
				}
			}

			return bookingConfig;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose()
		{
			
		}
	}
}
