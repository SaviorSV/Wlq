
namespace Wlq.Domain
{
	public class VenueGroupInfo : EntityBase
	{
		public string Name { get; set; }
		public long GroupId { get; set; }
		public int VenueType { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }

		public VenueGroupInfo()
		{
			Name = string.Empty;
		}
	}
}
