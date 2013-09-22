using System.Collections;
using System.Collections.Generic;

namespace Wlq.Domain
{
	public class VenueGroupInfo : Entity
	{
		public string Name { get; set; }
		public long GroupId { get; set; }
		public int VenueType { get; set; }

		public VenueGroupInfo()
		{
			Name = string.Empty;
		}
	}
}
