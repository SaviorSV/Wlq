
using Hanger.Common;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using System.Data.Entity;
using Wlq.Persistence;
using Wlq.Persistence.Migrations;
using Wlq.Service;

namespace Wlq.Test
{
	public class TestBase
	{
		#region Services

		private DatabaseContext _databaseContext;

		protected DatabaseContext DatabaseContext
		{
			get
			{
				if (_databaseContext == null)
					_databaseContext = new DatabaseContext();

				return _databaseContext;
			}
		}

		private ICommonService _commonService;

		protected ICommonService CommonService
		{
			get { return LocalServiceLocator.GetService<ICommonService>(); }
		}

		private IUserGroupService _userGroupService;

		protected IUserGroupService UserGroupService
		{
			get { return this.GetService<IUserGroupService>(ref _userGroupService); }
		}

		private IPostService _postService;

		protected IPostService PostService
		{
			get { return this.GetService<IPostService>(ref _postService); }
		}

		private TService GetService<TService>(ref TService service)
		{
			if (service == null)
			{
				service = LocalServiceLocator.GetService<TService>(
					new ParameterOverrides { { "databaseContext", this.DatabaseContext } }
				);
			}

			return service;
		}

		#endregion

		[SetUp]
		protected void TestSetUp()
		{
			Hanger.Common.HangerFramework.Start();

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
		}

		[TearDown]
		protected void TestTearDown()
		{
			Hanger.Common.HangerFramework.End();

			_databaseContext.Dispose();
		}
	}
}
