using System;
using System.ComponentModel;

namespace Wlq.Domain
{
	[Flags]
	public enum BookingType
	{
		[Description("网上预约")]
		Online = 1,

		[Description("电话预约")]
		Phone = 2,

		[Description("现场预约")]
		Spot = 4
	}
}
