
namespace Wlq.Domain
{
	public class UserGroupInfo : EntityBase, IUserGroupRelation
	{
		public long UserId { get; set; }
		public long GroupId { get; set; }
	}
}
