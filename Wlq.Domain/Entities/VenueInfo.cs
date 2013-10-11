
namespace Wlq.Domain
{
	public class VenueInfo : EntityBase
	{
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public long GroupId { get; set; }
		public long VenueGroupId { get; set; }
		public bool IsSuspend { get; set; }

		public VenueInfo()
		{
			Name = string.Empty;
			Phone = string.Empty;
			Address = string.Empty;
		}
	}
}
