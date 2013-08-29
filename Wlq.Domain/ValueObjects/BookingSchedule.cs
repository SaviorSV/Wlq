using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wlq.Domain
{
	public class BookingSchedule
	{
		public DateTime Date { get; set; }
		public List<BookingPeriod> Periods { get; set; }

		public BookingSchedule() { }
	}
}
