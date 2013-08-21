using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Wlq.Domain;
using Hanger.Common;

namespace Wlq.Test.ServiceTests
{
	public class UserGroupServiceTest : TestBase
	{
		[Test]
		public void AddSuperAdmin()
		{
			var user = new UserInfo();

			user.LoginName = "admin";
			user.Password = "111111".ToMd5();
			user.Role = (int)RoleLevel.SuperAdmin;

			var result = UserGroupService.AddUser(user);

			Assert.IsTrue(result);
		}
	}
}
