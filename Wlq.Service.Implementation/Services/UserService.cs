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
	public class UserService : Disposable, IUserService
	{
		private readonly DatabaseContext _databaseContext;

		public UserService(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		#region public methods

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

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose()
		{
			
		}
	}
}
