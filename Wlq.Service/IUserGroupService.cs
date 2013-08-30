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
		UserInfo GetUser(string loginName);
		bool AddUser(UserInfo user);
		bool UpdateUser(UserInfo user);
		bool DeleteUser(long userId);
		bool ChangePassword(UserInfo user, string newPassword);
		bool Login(string loginName, string password, bool isAdmin);

		#endregion

		#region Group

		IEnumerable<GroupInfo> GetGroupsByUser(long userId);
		IEnumerable<GroupInfo> GetGroupsByManager(long userId, RoleLevel role);
		IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId);
		GroupInfo GetGroup(long groupId);
		bool AddGroup(GroupInfo group);
		bool UpdateGroup(GroupInfo group);
		bool DeleteGroup(long groupId);

		#endregion

		#region GroupManager

		IEnumerable<UserInfo> GetManagersByGroup(long groupId);
		bool AddManagerToGroup(long userId, long groupId);
		bool RemoveManagerFromGroup(long userId, long groupId);
		bool IsManagerInGroup(long userId, long groupId);

		#endregion

		#region UserGroup

		bool AddUserToGroup(long userId, long groupId);
		bool RemoveUserFromGroup(long userId, long groupId);
		bool IsUserInGroup(long userId, long groupId);

		#endregion
	}
}
