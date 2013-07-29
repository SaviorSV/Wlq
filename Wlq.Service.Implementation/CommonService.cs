using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hanger.Common;
using Wlq.Service;

namespace Wlq.Service.Implementation
{
	public class CommonService : Disposable, ICommonService
	{
		public CommonService()
		{
			//Hanger.Common.LocalLoggingService.Info("CommonService()");
		}

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose()
		{
			
		}
	}
}
