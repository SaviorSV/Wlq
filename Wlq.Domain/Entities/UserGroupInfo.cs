
namespace Wlq.Domain
{
	public class UserGroupInfo : Entity, IUserGroupRelation
	{
		public long UserId { get; set; }
		public long GroupId { get; set; }
	}
}
