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
		AddUserResult AddUser(UserInfo user);
		bool UpdateUser(UserInfo user);
		bool DeleteUser(long userId);
		ChangePasswordResult ChangePassword(UserInfo user, string oldPassword, string newPassword);
		bool ResetPassword(long userId, string newPassword);
		bool Login(string loginName, string password, bool isAdmin);
		bool LoginByCode(string code);

		#endregion

		#region Group

		IEnumerable<GroupInfo> GetCircles(int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<GroupInfo> GetGroupsByUser(long userId);
		IEnumerable<GroupInfo> GetGroupsByManager(UserInfo manager);
		IEnumerable<GroupInfo> GetGroupTreeByManager(UserInfo manager);
		IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId);
		IEnumerable<GroupInfo> GetGroupTreeByParent(long parentGroupId, UserInfo manager, string keyword);
		GroupInfo GetGroup(long groupId, bool fromCache);
		bool AddGroup(GroupInfo group);
		bool UpdateGroup(GroupInfo group);
		bool DeleteGroup(long groupId);

		#endregion

		#region GroupManager

		IEnumerable<UserInfo> GetManagersByGroup(long groupId);
		IEnumerable<UserInfo> GetManagersByGroupTree(long groupId, UserInfo manager, string keyword);
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
