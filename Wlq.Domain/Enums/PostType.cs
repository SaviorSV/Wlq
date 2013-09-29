using System.ComponentModel;

namespace Wlq.Domain
{
	public enum PostType
	{
		[Description("全部")]
		All = 0,

		[Description("活动")]
		Activity = 1,

		[Description("课程")]
		Course = 2,

		[Description("场地")]
		Venue = 3,

		[Description("健康")]
		Health = 4
	}
}
