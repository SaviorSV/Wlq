using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Service;
using Hanger.Common;

namespace Wlq.Web.Controllers
{
    public class BaseController : Controller
	{
		#region Services

		private static ICommonService _commonService;

		protected static ICommonService CommonService
		{
			get 
			{
				if (_commonService == null)
				{
					_commonService = LocalServiceLocator.GetService<ICommonService>();
				}

				return _commonService;
			}
		}

		#endregion

		public BaseController()
		{
 
		}

		protected override void Dispose(bool disposing)
		{
			
			base.Dispose(disposing);
		}
    }
}
