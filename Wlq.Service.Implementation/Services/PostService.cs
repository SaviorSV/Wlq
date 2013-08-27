using System;
using System.Collections.Generic;
using System.Linq;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Persistence;
using Wlq.Service;

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

		public bool SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs)
		{
			if (venue == null || configs == null)
			{
				return false;
			}

			var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);
			var oldConfig = venueConfigRepository.GetAll().Where(v => v.VenueId == venue.Id).ToList();

			//traverse sunday to saturday
			foreach (var day in configs.Keys)
			{
				if (configs[day] != null)
				{
					//visit each period in the day
					foreach (var period in configs[day])
					{
						var old = oldConfig.FirstOrDefault(c =>
							c.DaysOfWeek == (int)day &&
							c.BegenTime == period.BeginTime &&
							c.EndTime == period.EndTime &&
							c.LimitNumber == period.LimitNumber);

						//if the same period is not in the database, insert item
						if (old == null)
						{
							var newConfig = new VenueConfigInfo
							{
								VenueId = venue.Id,
								DaysOfWeek = (int)day,
								BegenTime = period.BeginTime,
								EndTime = period.EndTime,
								LimitNumber = period.LimitNumber
							};

							venueConfigRepository.Add(newConfig);
						}
						//remove the existing item from list
						else
						{
							oldConfig.Remove(old);
						}
					}
				}
			}

			//delete the rest items in list from database
			foreach (var restOld in oldConfig)
			{
				venueConfigRepository.DeleteById(restOld.Id);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		public BookingConfig GetVenueConfigs(long venueId)
		{
			var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);
			var configs = venueConfigRepository.GetAll().Where(v => v.VenueId == venueId);
			var bookingConfig = new BookingConfig();

			foreach (var config in configs)
			{
				try
				{
					bookingConfig.AddPeriod((DayOfWeek)config.DaysOfWeek, config.BegenTime, config.EndTime, config.LimitNumber);
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
