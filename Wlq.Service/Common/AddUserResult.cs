using System.ComponentModel;

namespace Wlq.Service
{
	public enum AddUserResult
	{
		[Description("注册成功")]
		Success = 1,

		[Description("用户名已存在")]
		Exist = 2,

		[Description("注册异常")]
		Error = 3
	}
}
