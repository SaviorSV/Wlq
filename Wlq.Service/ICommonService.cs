using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface ICommonService : IDisposable
	{
		FileUploadResult UploadTempFile(long userId, string fileType);
	}
}
