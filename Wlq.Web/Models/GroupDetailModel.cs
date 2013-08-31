using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class GroupDetailModel
	{
		public GroupInfo Group { get; set; }
		public bool IsFollowing { get; set; }
		public IEnumerable<PostModel> Posts { get; set; }
	}
}