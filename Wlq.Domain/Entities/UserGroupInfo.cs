
namespace Wlq.Domain
{
	public class UserGroupInfo : Entity
	{
		public long UserId { get; set; }
		public long GroupId { get; set; }
		public bool IsManager { get; set; }
	}
}
