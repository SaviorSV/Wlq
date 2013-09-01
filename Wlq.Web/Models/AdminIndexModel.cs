using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class AdminIndexModel
	{
		public IEnumerable<PostInfo> ActivityList { get; set; }
		public IEnumerable<PostInfo> CourseList { get; set; }
		public IEnumerable<PostInfo> VenueList { get; set; }
	}
}