using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using Hanger.Caching;
using Hanger.Common;
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
			return base.RepositoryProvider<UserInfo>().GetById(userId);
		}

		public bool AddUser(UserInfo user)
		{
			var userRepository = base.RepositoryProvider<UserInfo>();
			var existUser = userRepository.Entities
				.FirstOrDefault(u => u.LoginName == user.LoginName);

			if (existUser != null)
			{
				return false;
			}

			return userRepository.Add(user, true) > 0;
		}

		public bool UpdateUser(UserInfo user)
		{
			return base.RepositoryProvider<UserInfo>().Update(user, true) > 0;
		}

		public bool DeleteUser(long userId)
		{
			return base.RepositoryProvider<UserInfo>().DeleteById(userId, true) > 0;
		}

		public ChangePasswordResult ChangePassword(UserInfo user, string oldPassword, string newPassword)
		{
			var oldHash = oldPassword.ToMd5();

			if (oldHash != user.Password)
			{
				return ChangePasswordResult.OldPasswordWrong;
			}

			user.Password = newPassword.ToMd5();

			return this.UpdateUser(user)
				? ChangePasswordResult.Success 
				: ChangePasswordResult.Error;
		}

		public bool ResetPassword(long userId, string newPassword)
		{
			var userRepository = base.RepositoryProvider<UserInfo>();
			var user = userRepository.GetById(userId);

			if (user == null)
			{
				return false;
			}

			user.Password = newPassword.ToMd5();

			return userRepository.Update(user, true) > 0;
		}

		public bool Login(string loginName, string password, bool isAdmin)
		{
			var hashedPassword = password.ToMd5();

			var user = base.RepositoryProvider<UserInfo>().Entities
				.FirstOrDefault(u => u.LoginName == loginName && u.Password == hashedPassword && (!isAdmin || u.Role > (int)RoleLevel.Normal));

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

		public IEnumerable<GroupInfo> GetGroupsByManager(long userId, RoleLevel role)
		{
			if (role == RoleLevel.SuperAdmin)
			{
				return this.GetGroupsByParent(0);
			}
			else
			{
				return this.GetGroupsByRelation<GroupManagerInfo>(userId);
			}
		}

		public IEnumerable<GroupInfo> GetGroupsByParent(long parentGroupId)
		{
			return base.RepositoryProvider<GroupInfo>().Entities
				.Where(g => g.ParentGroupId == parentGroupId);
		}

		public IEnumerable<GroupInfo> GetCircles(int pageIndex, int pageSize, out int totalNumber)
		{
			return base.RepositoryProvider<GroupInfo>().Entities
				.Where(g => g.ParentGroupId > 0)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public GroupInfo GetGroup(long groupId, bool fromCache)
		{
			var key = string.Format("Wlq.Domain.GroupInfo.{0}", groupId);
			var group = fromCache ? CacheHelper<GroupInfo>.Get(key) : null;

			if (group == null)
			{
				group = base.RepositoryProvider<GroupInfo>().GetById(groupId);

				if (group != null && fromCache)
				{
					CacheHelper<GroupInfo>.Set(key, group, new TimeSpan(0, 15, 0));
				}
			}

			return group;
		}

		public bool AddGroup(GroupInfo group)
		{
			return base.RepositoryProvider<GroupInfo>().Add(group, true) > 0;
		}

		public bool UpdateGroup(GroupInfo group)
		{
			var success = base.RepositoryProvider<GroupInfo>().Update(group, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.GroupInfo.{0}", group.Id);

				CacheHelper<GroupInfo>.Set(key, group, new TimeSpan(0, 15, 0));
			}

			return success;
		}

		public bool DeleteGroup(long groupId)
		{
			var success = base.RepositoryProvider<GroupInfo>().DeleteById(groupId, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.GroupInfo.{0}", groupId);

				CacheHelper<GroupInfo>.RemoveKey(key);
			}

			return success;
		}

		#endregion

		#region GroupManager

		public IEnumerable<UserInfo> GetManagersByGroup(long groupId)
		{
			return this.GetUsersByRelation<GroupManagerInfo>(groupId);
		}

		public bool AddManagerToGroup(long userId, long groupId)
		{
			return this.AddRelation<GroupManagerInfo>(userId, groupId);
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
			where TEntity : Entity, IUserGroupRelation
		{
			var relations = base.RepositoryProvider<TEntity>().Entities
				.Where(r => r.GroupId == groupId)
				.Select(r => r.UserId);

			return base.RepositoryProvider<UserInfo>().Entities
				.Where(u => relations.Contains(u.Id));
		}

		private IEnumerable<GroupInfo> GetGroupsByRelation<TEntity>(long userId)
			where TEntity : Entity, IUserGroupRelation
		{
			var relations = base.RepositoryProvider<TEntity>().Entities
				.Where(r => r.UserId == userId)
				.Select(r => r.GroupId);

			return base.RepositoryProvider<GroupInfo>().Entities
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

			var group = this.GetGroup(groupId, true);

			if (group == null)
				return false;

			var relation = new TEntity
			{
				UserId = userId,
				GroupId = groupId
			};

			return base.RepositoryProvider<TEntity>().Add(relation, true) > 0;
		}

		private bool RemoveRelations<TEntity>(long userId, long groupId)
			where TEntity : Entity, IUserGroupRelation
		{
			var relationRepository = base.RepositoryProvider<TEntity>();
			var relations = relationRepository.Entities
				.Where(ug => ug.UserId == userId && ug.GroupId == groupId);

			foreach (var relation in relations)
			{
				relationRepository.Delete(relation, false);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		private bool HasRelation<TEntity>(long userId, long groupId)
			where TEntity : Entity, IUserGroupRelation
		{
			var relation = base.RepositoryProvider<TEntity>().Entities
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
