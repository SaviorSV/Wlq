using System;
using System.Collections.Generic;

using Wlq.Domain;
using Wlq.Service.Utility;

namespace Wlq.Service
{
	public interface ICommonService : IDisposable
	{
		FileUploadResult UploadTempFile(long userId, string fileType);
		void CleanTempFile(long userId);
		void SaveLogo(long userId, long groupId);

		string GetPostTypeName(PostType type);
	}
}
