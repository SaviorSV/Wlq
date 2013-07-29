using System.Web.Mvc;

using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Persistence;
using Wlq.Service;

namespace Wlq.Web.Controllers
{
    public class BaseController : Controller
	{
		#region Services & Database Context

		private DatabaseContext _databaseContext;

		private DatabaseContext DatabaseContext
		{
			get
			{
				if (_databaseContext == null)
				{
					_databaseContext = new DatabaseContext();
				}

				return _databaseContext;
			}
		}

		private ICommonService _commonService;

		protected ICommonService CommonService
		{
			get 
			{
				return this.GetService<ICommonService>(ref _commonService);
			}
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

		public BaseController()
		{
 
		}

		protected override void Dispose(bool disposing)
		{
			if (_databaseContext != null)
				_databaseContext.Dispose();

			base.Dispose(disposing);
		}
    }
}
