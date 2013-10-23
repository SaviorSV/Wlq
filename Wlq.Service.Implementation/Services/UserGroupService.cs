using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using Hanger.Caching;
using Hanger.Common;
using Hanger.Utility;
using Microsoft.Practices.Unity;
using Wlq.Domain;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class UserGroupService : ServiceBase, IUserGroupService
	{
		[InjectionConstructor]
		public UserGroupService(DatabaseContext databaseContext)
			: base(databaseContext)
		{ }

		#region User

		public UserInfo GetUser(long userId)
		{
			return base.GetRepository<UserInfo>().GetById(userId);
		}

		public AddUserResult AddUser(UserInfo user)
		{
			var userRepository = base.GetRepository<UserInfo>();
			var existUser = userRepository.Entities
				.FirstOrDefault(u => u.LoginName == user.LoginName);

			if (existUser != null)
			{
				return AddUserResult.Exist;
			}

			return userRepository.Add(user, true) > 0
				? AddUserResult.Success : AddUserResult.Error;
		}

		public bool UpdateUser(UserInfo user)
		{
			return base.GetRepository<UserInfo>().Update(user, true) > 0;
		}

		public bool DeleteUser(long userId)
		{
			return base.GetRepository<UserInfo>().DeleteById(userId, true) > 0;
		}

		public ChangePasswordResult ChangePassword(UserInfo user, string oldPassword, string newPassword)
		{
			var oldHash = StringHelper.GetMd5(oldPassword);

			if (oldHash != user.Password)
			{
				return ChangePasswordResult.OldPasswordWrong;
			}

			user.Password = StringHelper.GetMd5(newPassword);

			return base.GetRepository<UserInfo>().Update(user, true) > 0
				? ChangePasswordResult.Success 
				: ChangePasswordResult.Error;
		}

		public bool ResetPassword(long userId, string newPassword)
		{
			var userRepository = base.GetRepository<UserInfo>();
			var user = userRepository.GetById(userId);

			if (user == null)
			{
				return false;
			}

			user.Password = StringHelper.GetMd5(newPassword);

			return userRepository.Update(user, true) > 0;
		}

		public bool Login(string loginName, string password, bool isAdmin)
		{
			var hashedPassword = StringHelper.GetMd5(password);

			var user = base.GetRepository<UserInfo>().Entities
				.FirstOrDefault(u => u.LoginName == loginName && u.Password == hashedPassword && (!isAdmin || u.Role > (int)RoleLevel.Normal));

			return this.Login(user);
		}

		public bool LoginByCode(string code)
		{
			var user = base.GetRepository<UserInfo>().Entities
				.FirstOrDefault(u => u.Code == code);

			return this.Login(user);
		}

		private bool Login(UserInfo user)
		{
			if (user == null)
				return false;

			var ticket = new FormsAuthenticationTicket(
				1,
				user.Id.ToString(),
				DateTime.Now,
				DateTime.Now.AddMinutes(30),
				true,
				user.Role.ToString(),
				FormsAuthentication.FormsCookiePath);

			var encTicket = FormsAuthentication.Encrypt(ticket);

			HttpContext.Current.Response.Cookies
				.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

			return true;
		}

		#endregion

		#region Group

		public IEnumerable<GroupInfo> GetGroupsByUser(long userId)
		{
			return this.GetGroupsByRelation<UserGroupInfo>(userId);
		}

		public IEnumerable<GroupInfo> GetGroupsByManager(UserInfo manager)
		{
			if (manager.Role == (int)RoleLevel.SuperAdmin)
			{
				return base.GetRepository<GroupInfo>().Entities
					.Where(g => g.GroupType == (int)GroupType.Department);
			}
			else
			{
				return this.GetGroupsByRelation<GroupManagerInfo>(manager.Id);
			}
		}

		public IEnumerable<GroupInfo> GetGroupTreeByManager(UserInfo manager)
		{
			var groups = base.GetRepository<GroupInfo>().Entities;

			if (manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var parentGroupIds = base.GetRepository<GroupManagerInfo>().Entities
					.Where(r => r.UserId == manager.Id)
					.Select(r => r.GroupId);

				groups = groups.Where(g => parentGroupIds.Contains(g.Id) || parentGroupIds.Contains(g.ParentGroupId));
			}

			return groups
				.OrderBy(g => g.GroupType);
		}

		public IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId)
		{
			return base.GetRepository<GroupInfo>().Entities
				.Where(g => g.ParentGroupId == parentGroupId);
		}

		public IEnumerable<GroupInfo> GetGroupTreeByParent(long parentGroupId, UserInfo manager, string keyword)
		{
			var groups = base.GetRepository<GroupInfo>().Entities;

			if (parentGroupId > 0)
			{
				groups = groups.Where(g => g.ParentGroupId == parentGroupId);
			}
			else if (manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var parentGroupIds = base.GetRepository<GroupManagerInfo>().Entities
					.Where(r => r.UserId == manager.Id)
					.Select(r => r.GroupId);

				groups = groups.Where(g => parentGroupIds.Contains(g.ParentGroupId));
			}

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				groups = groups.Where(g => g.Name.Contains(keyword));
			}

			return groups
				.OrderBy(g => g.GroupType);
		}

		public IEnumerable<GroupInfo> GetCircles(int pageIndex, int pageSize, out int totalNumber)
		{
			return base.GetRepository<GroupInfo>().Entities
				.Where(g => g.ParentGroupId > 0)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public GroupInfo GetGroup(long groupId, bool fromCache)
		{
			var key = string.Format("Wlq.Domain.GroupInfo.{0}", groupId);

			return CacheManager.Get<GroupInfo>(fromCache, key, new TimeSpan(0, 15, 0),
				() =>
				{
					return base.GetRepository<GroupInfo>().GetById(groupId);
				});
		}

		public bool AddGroup(GroupInfo group)
		{
			return base.GetRepository<GroupInfo>().Add(group, true) > 0;
		}

		public bool UpdateGroup(GroupInfo group)
		{
			var success = base.GetRepository<GroupInfo>().Update(group, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.GroupInfo.{0}", group.Id);

				CacheManager.Update<GroupInfo>(key, group, new TimeSpan(0, 15, 0));
			}

			return success;
		}

		public bool DeleteGroup(long groupId)
		{
			var repository = base.GetRepository<GroupInfo>();
			var success = repository.DeleteById(groupId, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.GroupInfo.{0}", groupId);

				CacheManager.RemoveKey(key);

				var subGroups = repository.Entities
					.Where(g => g.ParentGroupId == groupId);

				foreach (var group in subGroups)
				{
					repository.Delete(group, false);

					key = string.Format("Wlq.Domain.GroupInfo.{0}", group.Id);

					CacheManager.RemoveKey(key);
				}

				_databaseContext.SaveChanges();
			}

			return success;
		}

		#endregion

		#region GroupManager

		public IEnumerable<UserInfo> GetManagersByGroup(long groupId)
		{
			return this.GetUsersByRelation<GroupManagerInfo>(groupId);
		}

		public IEnumerable<UserInfo> GetManagersByGroupTree(long groupId, UserInfo manager, string keyword)
		{
			var groupManagerRepository = base.GetRepository<GroupManagerInfo>();
			var groups = base.GetRepository<GroupInfo>().Entities;

			if (groupId > 0)
			{
				groups = groups.Where(g => g.Id == groupId || g.ParentGroupId == groupId);
			}
			else if (manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var manageGroupIds = groupManagerRepository.Entities
					.Where(gm => gm.UserId == manager.Id)
					.Select(gm => gm.GroupId);

				groups = groups.Where(g => manageGroupIds.Contains(g.ParentGroupId));
			}

			var managerIds = groupManagerRepository.Entities
				.Where(gm => groups.Select(g => g.Id).Contains(gm.GroupId))
				.Select(m => m.UserId);

			var managers = base.GetRepository<UserInfo>().Entities
				.Where(u => managerIds.Contains(u.Id));

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				managers = managers.Where(u => u.LoginName.Contains(keyword));
			}

			return managers;
		}

		public bool AddManagerToGroup(long userId, long groupId)
		{
			return this.AddRelation<GroupManagerInfo>(userId, groupId);
		}

		public bool RemoveManager(long userId)
		{
			var success = base.GetRepository<UserInfo>().DeleteById(userId, true) > 0;

			if (success)
			{
				var relationRepository = base.GetRepository<GroupManagerInfo>();
				var relations = relationRepository.Entities
					.Where(ug => ug.UserId == userId);

				foreach (var relation in relations)
				{
					relationRepository.Delete(relation, false);
				}

				_databaseContext.SaveChanges();
			}

			return success;
		}

		public bool RemoveManagerFromGroup(long userId, long groupId)
		{
			return this.RemoveRelations<GroupManagerInfo>(userId, groupId);
		}

		public bool IsManagerInGroup(long userId, long groupId)
		{
			return this.HasRelation<GroupManagerInfo>(userId, groupId);
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
			where TEntity : EntityBase, IUserGroupRelation
		{
			var relations = base.GetRepository<TEntity>().Entities
				.Where(r => r.GroupId == groupId)
				.Select(r => r.UserId);

			return base.GetRepository<UserInfo>().Entities
				.Where(u => relations.Contains(u.Id));
		}

		private IEnumerable<GroupInfo> GetGroupsByRelation<TEntity>(long userId)
			where TEntity : EntityBase, IUserGroupRelation
		{
			var relations = base.GetRepository<TEntity>().Entities
				.Where(r => r.UserId == userId)
				.Select(r => r.GroupId);

			return base.GetRepository<GroupInfo>().Entities
				.Where(g => relations.Contains(g.Id));
		}

		private bool AddRelation<TEntity>(long userId, long groupId)
			where TEntity : EntityBase, IUserGroupRelation, new()
		{
			if (this.HasRelation<TEntity>(userId, groupId))
				return true;

			var user = base.GetRepository<UserInfo>().GetById(userId);

			if (user == null)
				return false;

			var group = base.GetRepository<GroupInfo>().GetById(groupId);

			if (group == null)
				return false;

			var relation = new TEntity
			{
				UserId = userId,
				GroupId = groupId
			};

			return base.GetRepository<TEntity>().Add(relation, true) > 0;
		}

		private bool RemoveRelations<TEntity>(long userId, long groupId)
			where TEntity : EntityBase, IUserGroupRelation
		{
			var relationRepository = base.GetRepository<TEntity>();
			var relations = relationRepository.Entities
				.Where(ug => ug.UserId == userId && ug.GroupId == groupId);

			foreach (var relation in relations)
			{
				relationRepository.Delete(relation, false);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		private bool HasRelation<TEntity>(long userId, long groupId)
			where TEntity : EntityBase, IUserGroupRelation
		{
			var relation = base.GetRepository<TEntity>().Entities
				.FirstOrDefault(ug => ug.GroupId == groupId && ug.UserId == userId);

			return relation != null;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
