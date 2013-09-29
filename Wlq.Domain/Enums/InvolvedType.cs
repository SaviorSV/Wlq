﻿using System;
using System.ComponentModel;

namespace Wlq.Domain
{
	[Flags]
	public enum InvolvedType
	{
		[Description("-")]
		None = 0,

		[Description("观摩类")]
		Observer = 1,

		[Description("参与互动类")]
		Participant = 2,

		[Description("支援服务类")]
		Volunteer = 4,
	}
}