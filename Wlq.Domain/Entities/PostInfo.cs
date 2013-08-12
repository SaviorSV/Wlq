
namespace Wlq.Domain
{
	public class PostInfo : Entity
	{
		public string Title { get; set; }
		public string Content { get; set; }
		public string Image { get; set; }
		public long GroupId { get; set; }
		public int GroupType { get; set; }
	}
}
