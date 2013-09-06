using System;
using System.Collections.Generic;

namespace Wlq.Domain
{
	public class BookingSchedule
	{
		public string Date { get; set; }
		public List<BookingPeriod> Periods { get; set; }

		public BookingSchedule() { }
	}
}
