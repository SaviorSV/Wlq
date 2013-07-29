using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hanger.Common;
using Wlq.Service;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class CommonService : Disposable, ICommonService
	{
		private readonly DatabaseContext _databaseContext;

		public CommonService(DatabaseContext databaseContext)
		{
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
