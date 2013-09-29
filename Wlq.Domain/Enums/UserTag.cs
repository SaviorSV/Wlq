using System;
using System.ComponentModel;

namespace Wlq.Domain
{
	[Flags]
	public enum UserTag
	{
		[Description("党员")]
		PartyMember = 1,

		[Description("志愿者")]
		Volunteer = 2
	}
}
