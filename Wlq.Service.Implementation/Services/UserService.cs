using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hanger.Common;
using Wlq.Service;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class UserService : Disposable, IUserService
	{
		private readonly DatabaseContext _databaseContext;

		private UserService() { }

		public UserService(DatabaseContext databaseContext)
		{
			if (databaseContext == null)
			{
				throw new ArgumentNullException("databaseContext");
			}

			_databaseContext = databaseContext;
		}

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose()
		{
			
		}
	}
}
