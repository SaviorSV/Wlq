
using System.ComponentModel;

namespace Wlq.Domain
{
	public enum VenueType
	{
		[Description("全部")]
		All = 0,

		[Description("体育场馆")]
		Sports = 1,

		[Description("文化场馆")]
		Cultural = 2,

		[Description("教育场馆")]
		Education = 3
	}
}
