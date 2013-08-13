using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Hanger.Common;
using Wlq.Service;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class PostService : Disposable, IPostService
	{
		private readonly DatabaseContext _databaseContext;

		public PostService(DatabaseContext databaseContext)
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
