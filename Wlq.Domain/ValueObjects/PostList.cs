using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wlq.Domain
{
	public class PostList
	{
		public int TotalNumber { get; set; }
		public IEnumerable<PostInfo> List { get; set; }
	}
}
