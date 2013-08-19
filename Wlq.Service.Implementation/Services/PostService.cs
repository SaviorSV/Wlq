using System;
using System.Linq;

using Hanger.Common;
using Wlq.Service;
using Wlq.Persistence;
using Wlq.Domain;

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
