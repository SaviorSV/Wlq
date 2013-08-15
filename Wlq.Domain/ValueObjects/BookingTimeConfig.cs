using System;
using System.Collections.Generic;

using Hanger.Common;

namespace Wlq.Domain
{
	public class BookingTimeConfig : Dictionary<DayOfWeek, List<Period>>
	{
		public BookingTimeConfig()
		{
			foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
			{
				this.Add(day, new List<Period>());
			}
		}

		public void AddPeriod(DayOfWeek day, Period period)
		{
			if (HasOverlappingPeriods(day, period))
			{
				throw new Exception("存在重叠时间段!");
			}

			this[day].Add(period);
		}

		public bool HasOverlappingPeriods(DayOfWeek day, Period period)
		{
			if (this[day].Count == 0)
				return false;

			foreach (var existPeriod in this[day])
			{
				if (existPeriod.IsOverlapping(period))
					return true;
			}

			return false;
		}

		public string ToJson()
		{
			return this.ObjectToJson();
		}
	}

	public class Period
	{
		public TimeOfDay From { get; set; }
		public TimeOfDay To { get; set; }

		public Period(TimeOfDay from, TimeOfDay to)
		{
			if (from.CompareTo(to) >= 0)
				throw new Exception("起止时间异常!");

			this.From = from;
			this.To = to;
		}

		public bool IsOverlapping(Period other)
		{
			if (other == null)
				return false;

			// other.From >= this.To || other.To <= this.From
			if ((other.From.CompareTo(this.To) >= 0 || other.To.CompareTo(this.From) <= 0))
				return false;

			return true;
		}
	}

	public class TimeOfDay : IComparable<TimeOfDay>
	{
		public int Hour { get; set; }
		public int Minute { get; set; }

		public TimeOfDay(int hour, int minute)
		{
			if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
				throw new Exception("时间设置异常!");

			this.Hour = hour;
			this.Minute = minute;
		}

		public int CompareTo(TimeOfDay other)
		{
			if (other == null)
				return 1;

			var thisTime = this.Hour * 100 + this.Minute;
			var otherTime = other.Hour * 100 + other.Minute;

			if (thisTime == otherTime)
				return 0;
			else
				return thisTime > otherTime ? 1 : -1;
		}
	}
}
