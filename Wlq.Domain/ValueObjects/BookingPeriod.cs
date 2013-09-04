
namespace Wlq.Domain
{
	public class BookingPeriod
	{
		public int BeginTime { get; set; }
		public int EndTime { get; set; }
		public int LimitNumber { get; set; }
		public int BookingNumber { get; set; }
		public long VenueConfigId { get; set; }
		public bool IsBooked { get; set; }

		public BookingPeriod(int begin, int end, int limit, long venueConfigId)
		{
			this.BeginTime = begin;
			this.EndTime = end;
			this.LimitNumber = limit;
			this.VenueConfigId = venueConfigId;
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
