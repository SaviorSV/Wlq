using System;
using System.Collections.Generic;

using Hanger.Common;

namespace Wlq.Domain
{
	public class BookingConfig
	{
		private Dictionary<DayOfWeek, List<BookingPeriod>> _timeConfig;

		public BookingConfig()
		{
			_timeConfig = new Dictionary<DayOfWeek, List<BookingPeriod>>();

			foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
			{
				this._timeConfig.Add(day, new List<BookingPeriod>());
			}
		}

		public List<BookingPeriod> this[DayOfWeek key]
		{
			get
			{
				return _timeConfig[key];
			}
		}

		public void AddPeriod(DayOfWeek day, int begin, int end, int limit, long venueConfigId)
		{
			if (!Enum.IsDefined(typeof(DayOfWeek), day))
			{
				throw new Exception("DayOfWeek枚举异常!");
			}

			var period = new BookingPeriod(begin, end, limit, venueConfigId);

			if (HasOverlappingPeriods(day, period))
			{
				throw new Exception("存在重叠时间段!");
			}

			this._timeConfig[day].Add(period);
		}

		public string ToJson()
		{
			return _timeConfig.ObjectToJson();
		}

		private bool HasOverlappingPeriods(DayOfWeek day, BookingPeriod period)
		{
			if (this._timeConfig[day].Count == 0)
			{
				return false;
			}

			foreach (var existPeriod in this._timeConfig[day])
			{
				if (existPeriod.IsOverlapping(period))
				{
					return true;
				}
			}

			return false;
		}
	}
}
