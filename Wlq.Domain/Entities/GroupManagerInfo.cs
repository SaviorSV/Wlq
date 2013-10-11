
namespace Wlq.Domain
{
	public class GroupManagerInfo : EntityBase, IUserGroupRelation
	{
		public long UserId { get; set; }
		public long GroupId { get; set; }
	}
}
