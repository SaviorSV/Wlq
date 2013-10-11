
namespace Wlq.Domain
{
	public class UserMessageInfo : EntityBase
	{
		public long UserId { get; set; }
		public long MessageId { get; set; }
		public bool IsRead { get; set; }
	}
}
