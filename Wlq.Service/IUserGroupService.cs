using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	/// <summary>
	/// User & Group
	/// </summary>
	public interface IUserGroupService : IDisposable
	{
		#region User

		UserInfo GetUser(long userId);
		UserInfo GetUser(string loginName, string password);
		bool AddUser(UserInfo user);
		bool UpdateUser(UserInfo user);
		bool DeleteUser(long userId);
		bool ChangePassword(UserInfo user, string newPassword); 

		#endregion

		#region Group

		IEnumerable<GroupInfo> GetGroupsByUser(long userId);
		IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId);
		GroupInfo GetGroup(long groupId);
		bool AddGroup(GroupInfo group);
		bool UpdateGroup(GroupInfo group);
		bool DeleteGroup(long groupId);

		#endregion

		#region UserGroup

		bool AddUserToGroup(long userId, long groupId, bool isManager);
		bool RemoveUserFromGroup(long userId, long groupId);

		#endregion
	}
}
