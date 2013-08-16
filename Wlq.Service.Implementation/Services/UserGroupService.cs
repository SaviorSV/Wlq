using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Persistence;
using Wlq.Service;

namespace Wlq.Service.Implementation
{
	public class UserGroupService : Disposable, IUserGroupService
	{
		private readonly DatabaseContext _databaseContext;

		public UserGroupService(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		#region User

		public UserInfo GetUser(long userId)
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);

			return userRepository.GetById(userId);
		}

		public UserInfo GetUser(string loginName, string password)
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);

			return userRepository.GetAll()
				.FirstOrDefault(u => u.LoginName == loginName && u.Password == password.ToMd5());
		}

		public bool AddUser(UserInfo user)
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);

			userRepository.Add(user);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool UpdateUser(UserInfo user)
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);

			userRepository.Update(user);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool DeleteUser(long userId)
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);

			userRepository.DeleteById(userId);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool ChangePassword(UserInfo user, string newPassword)
		{
			user.Password = newPassword.ToMd5();

			return this.UpdateUser(user);
		}

		#endregion

		#region Group

		public IEnumerable<GroupInfo> GetGroupsByUser(long userId)
		{
			var userGroups = _databaseContext.UserGroups.Where(ug => ug.UserId == userId).Select(ug => ug.GroupId);

			return _databaseContext.Groups
				.Where(g => userGroups.Contains(g.Id));
		}

		public IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId)
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);

			return groupRepository.GetAll()
				.Where(g => g.ParentGroupId == parentGroupId);
		}

		public GroupInfo GetGroup(long groupId)
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);

			return groupRepository.GetById(groupId);
		}

		public bool AddGroup(GroupInfo group)
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);

			groupRepository.Add(group);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool UpdateGroup(GroupInfo group)
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);

			groupRepository.Update(group);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool DeleteGroup(long groupId)
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);

			groupRepository.DeleteById(groupId);

			return _databaseContext.SaveChanges() > 0;
		}

		#endregion

		#region UserGroup

		public bool AddUserToGroup(long userId, long groupId, bool isManager)
		{
			var user = this.GetUser(userId);

			if (user == null)
				return false;

			var group = this.GetGroup(groupId);

			if (group == null)
				return false;

			var userGroup = new UserGroupInfo
			{
				UserId = userId,
				GroupId = groupId,
				IsManager = isManager
			};

			var userGroupRepository = new DatabaseRepository<UserGroupInfo>(_databaseContext);

			userGroupRepository.Add(userGroup);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool RemoveUserFromGroup(long userId, long groupId)
		{
			var userGroupRepository = new DatabaseRepository<UserGroupInfo>(_databaseContext);
			var userGroups = userGroupRepository.GetAll()
				.Where(ug => ug.UserId == userId && ug.GroupId == groupId);

			if (userGroups != null)
			{
				foreach (var userGroup in userGroups)
				{
					userGroupRepository.DeleteById(userGroup.Id);
				}
			}

			return _databaseContext.SaveChanges() > 0;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose()
		{
			
		}
	}
}
