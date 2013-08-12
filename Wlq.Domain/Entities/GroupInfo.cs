
namespace Wlq.Domain
{
	public class GroupInfo : Entity
	{
		public string Name { get; set; }
		public string Logo { get; set; }
		public long ParentGroup { get; set; }
		public int GroupType { get; set; }
	}
}
