using System;

namespace Wlq.Domain
{
	[Flags]
	public enum BookingType
	{
		Online = 1,

		Phone = 2,

		Spot = 4
	}
}
