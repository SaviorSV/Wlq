using System;
using System.Collections.Generic;

using Hanger.Common;

namespace Wlq.Domain
{
	public class BookingPeriod
	{
		public int BeginTime { get; set; }
		public int EndTime { get; set; }
		public int LimitNumber { get; set; }

		public BookingPeriod(int begin, int end, int limit)
		{
			if (begin.CompareTo(end) >= 0)
			{
				throw new Exception("起止时间异常!");
			}

			this.BeginTime = begin;
			this.EndTime = end;
			this.LimitNumber = limit;
		}

		public bool IsOverlapping(BookingPeriod other)
		{
			if (other == null)
			{
				return false;
			}

			// other.From >= this.To || other.To <= this.From
			if (other.BeginTime.CompareTo(this.EndTime) >= 0 || other.EndTime.CompareTo(this.BeginTime) <= 0)
			{
				return false;
			}

			return true;
		}
	}
}
