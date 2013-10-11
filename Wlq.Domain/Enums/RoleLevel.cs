using System.ComponentModel;

namespace Wlq.Domain
{
	public enum RoleLevel
	{
		[Description("普通用户")]
		Normal = 0,

		[Description("管理员")]
		Manager = 1,

		[Description("超级管理员")]
		SuperAdmin = 2
	}
}
