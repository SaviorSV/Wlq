﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Wlq.Domain;
using Hanger.Common;
using Hanger.Utility;
using Wlq.Service;

namespace Wlq.Test.ServiceTests
{
	public class UserGroupServiceTest : TestBase
	{
		[Test]
		public void AddSuperAdmin()
		{
			var user = new UserInfo();

			user.LoginName = "admin";
			user.Password = StringHelper.GetMd5("111111");
			user.Role = (int)RoleLevel.SuperAdmin;

			var result = UserGroupService.AddUser(user);

			Assert.AreEqual(AddUserResult.Success, result);
		}

		[Test]
		public void AddGroups()
		{
			var success = false;
			var groupNames = new string[] { "教育局", "文化局", "体育局", "卫生局" };

			foreach (var name in groupNames)
			{
				var group = new GroupInfo
				{
					Name = name,
					GroupType = (int)GroupType.Department
				};

				success = UserGroupService.AddGroup(group);

				if (!success)
					break;
			}

			Assert.IsTrue(success);
		}

		[Test]
		public void AddCircles()
		{
			var success = true;
			var groups = UserGroupService.GetGroupsByParent(0);

			if (groups != null && groups.Count() > 0)
			{
				var group = groups.First();

				for (int i = 0; i < 5; i++)
				{
					var circle = new GroupInfo
					{
						Name = "圈子" + i.ToString(),
						ParentGroupId = group.Id,
						GroupType = (int)GroupType.Circle
					};

					success = UserGroupService.AddGroup(circle);

					if (!success)
						break;
				}
			}

			Assert.IsTrue(success);
		}
	}
}
