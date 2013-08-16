﻿using System;
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
			return this.GetGroupsByRelation<UserGroupInfo>(userId);
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
			var userGroupRepository = new DatabaseRepository<UserGroupInfo>(_databaseContext);
			
			groupRepository.DeleteById(groupId);

			var userGroups = userGroupRepository.GetAll()
				.Where(ug => ug.GroupId == groupId);

			foreach (var userGroup in userGroups)
			{
				userGroupRepository.DeleteById(userGroup.Id);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		#endregion

		#region GroupManager

		public IEnumerable<GroupInfo> GetGroupsByManager(long userId)
		{
			return this.GetGroupsByRelation<GroupManagerInfo>(userId);
		}

		public IEnumerable<UserInfo> GetManagersByGroup(long groupId)
		{
			return this.GetUsersByRelation<GroupManagerInfo>(groupId);
		}

		public bool AddUserToGroupManager(long userId, long groupId)
		{
			return this.AddRelation<GroupManagerInfo>(userId, groupId);
		}

		public bool RemoveUserFromGroupManager(long userId, long groupId)
		{
			return this.RemoveRelations<GroupManagerInfo>(userId, groupId);
		}

		#endregion

		#region UserGroup

		public bool AddUserToGroup(long userId, long groupId)
		{
			return this.AddRelation<UserGroupInfo>(userId, groupId);
		}

		public bool RemoveUserFromGroup(long userId, long groupId)
		{
			return this.RemoveRelations<UserGroupInfo>(userId, groupId);
		}

		public bool IsUserInGroup(long userId, long groupId)
		{
			return this.HasRelation<UserGroupInfo>(userId, groupId);
		}

		#endregion

		#region Private

		private IEnumerable<UserInfo> GetUsersByRelation<TEntity>(long groupId)
			where TEntity : Entity, IUserGroupRelation
		{
			var userRepository = new DatabaseRepository<UserInfo>(_databaseContext);
			var relationRepository = new DatabaseRepository<TEntity>(_databaseContext);

			var relations = relationRepository.GetAll()
				.Where(r => r.GroupId == groupId).Select(r => r.UserId);

			return userRepository.GetAll()
				.Where(u => relations.Contains(u.Id));
		}

		private IEnumerable<GroupInfo> GetGroupsByRelation<TEntity>(long userId)
			where TEntity : Entity, IUserGroupRelation
		{
			var groupRepository = new DatabaseRepository<GroupInfo>(_databaseContext);
			var relationRepository = new DatabaseRepository<TEntity>(_databaseContext);

			var relations = relationRepository.GetAll()
				.Where(r => r.UserId == userId).Select(r => r.GroupId);

			return groupRepository.GetAll()
				.Where(g => relations.Contains(g.Id));
		}

		private bool AddRelation<TEntity>(long userId, long groupId)
			where TEntity : Entity, IUserGroupRelation, new()
		{
			if (this.HasRelation<TEntity>(userId, groupId))
				return true;

			var user = this.GetUser(userId);

			if (user == null)
				return false;

			var group = this.GetGroup(groupId);

			if (group == null)
				return false;

			var relation = new TEntity
			{
				UserId = userId,
				GroupId = groupId
			};

			var relationRepository = new DatabaseRepository<TEntity>(_databaseContext);

			relationRepository.Add(relation);

			return _databaseContext.SaveChanges() > 0;
		}

		private bool RemoveRelations<TEntity>(long userId, long groupId)
			where TEntity : Entity, IUserGroupRelation
		{
			var relationRepository = new DatabaseRepository<TEntity>(_databaseContext);
			var relations = relationRepository.GetAll()
				.Where(ug => ug.UserId == userId && ug.GroupId == groupId);

			foreach (var relation in relations)
			{
				relationRepository.DeleteById(relation.Id);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		private bool HasRelation<TEntity>(long userId, long groupId)
			where TEntity : Entity, IUserGroupRelation
		{
			var relationRepository = new DatabaseRepository<TEntity>(_databaseContext);
			var relation = relationRepository.GetAll()
				.FirstOrDefault(ug => ug.GroupId == groupId && ug.UserId == userId);

			return relation != null;
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
