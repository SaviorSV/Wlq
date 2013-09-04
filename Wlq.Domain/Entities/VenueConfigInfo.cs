
namespace Wlq.Domain
{
	public class VenueConfigInfo : Entity
	{
		public long VenueId { get; set; }
		public int DaysOfWeek { get; set; }
		public int BegenTime { get; set; }
		public int EndTime { get; set; }
		public int LimitNumber { get; set; }
	}
}
