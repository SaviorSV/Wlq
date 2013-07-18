using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Core.Services;

namespace Wlq.Core.Implementation
{
	public class RegisterServiceTask : RegisterServiceBootstrapperTask
	{
		public RegisterServiceTask(IUnityContainer container) : base(container) { }

		public override void Execute()
		{
			container.RegisterTypeAsSingleton<ICommonService, CommonService>();
		}

		protected override void InternalDispose()
		{
			container.Resolve<ICommonService>().Dispose();
		}
	}
}
