using System;

namespace Wlq.Service
{
	public interface ICommonService : IDisposable
	{
		FileUploadResult UploadTempFile(long userId, string fileType);
		FileUploadResult UploadImportFile();
		void CleanTempFile(long userId);
		void SaveGroupLogo(long userId, long groupId);
		void SavePostImage(long userId, long groupId, long postId);
		void SaveUserAvatar(long userId);
	}
}
