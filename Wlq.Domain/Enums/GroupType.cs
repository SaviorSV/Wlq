
using System.ComponentModel;

namespace Wlq.Domain
{
	public enum GroupType
	{
		[Description("全部")]
		All = 0,

		[Description("部门")]
		Department = 1,

		[Description("生活圈")]
		Circle = 2
	}
}
