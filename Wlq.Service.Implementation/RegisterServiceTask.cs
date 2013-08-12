﻿using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Service;

namespace Wlq.Service.Implementation
{
	public class RegisterServiceTask : RegisterServiceBootstrapperTask
	{
		public RegisterServiceTask(IUnityContainer container) : base(container) { }

		public override void Execute()
		{
			container.RegisterTypeAsPerResolve<ICommonService, CommonService>();
			container.RegisterTypeAsPerResolve<IPostService, PostService>();
			container.RegisterTypeAsPerResolve<IUserService, UserService>();
		}

		protected override void InternalDispose()
		{
			container.Resolve<ICommonService>().Dispose();
			container.Resolve<IPostService>().Dispose();
			container.Resolve<IUserService>().Dispose();
		}
	}
}
