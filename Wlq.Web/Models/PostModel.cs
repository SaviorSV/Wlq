
using Wlq.Domain;

namespace Wlq.Web.Models
{
	public class PostModel
	{
		public PostInfo Post { get; set; }
		public GroupInfo Group { get; set; }
		public bool IsBooked { get; set; }
	}
}