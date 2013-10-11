using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class AdminManagementModel
	{
		public IEnumerable<GroupInfo> Departments { get; set; }
		public IEnumerable<GroupInfo> Circles { get; set; }
	}
}