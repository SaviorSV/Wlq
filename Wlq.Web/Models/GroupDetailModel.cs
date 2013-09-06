using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class GroupDetailModel : GroupModel
	{
		public IEnumerable<PostModel> Posts { get; set; }
	}
}