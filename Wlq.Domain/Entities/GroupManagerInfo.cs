
namespace Wlq.Domain
{
	public class GroupManagerInfo : Entity, IUserGroupRelation
	{
		public long UserId { get; set; }
		public long GroupId { get; set; }
	}
}
