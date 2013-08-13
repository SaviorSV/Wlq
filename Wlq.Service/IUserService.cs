using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IUserService : IDisposable
	{
		UserInfo GetUser(long userId);
		UserInfo GetUser(string loginName, string password);
		bool AddUser(UserInfo user);
		bool UpdateUser(UserInfo user);
	}
}
