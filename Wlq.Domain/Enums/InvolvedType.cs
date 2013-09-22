using System;

namespace Wlq.Domain
{
	[Flags]
	public enum InvolvedType
	{
		Observer = 1,

		Participant = 2,

		Volunteer = 4,
	}
}
