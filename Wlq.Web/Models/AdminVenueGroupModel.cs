using System.Collections.Generic;
using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class AdminVenueGroupModel
	{
		public VenueGroupInfo VenueGroup { get; set; }
		public IEnumerable<GroupInfo> Groups { get; set; }
		public IEnumerable<VenueInfo> Venues { get; set; }
	}
}