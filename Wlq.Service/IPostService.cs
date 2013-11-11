using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IPostService : IDisposable
	{
		#region Venue

		IEnumerable<VenueInfo> GetVenuesByVenueGroup(long venueGroupId);
		IEnumerable<VenueInfo> GetVenuesByVenueGroupNotSuspended(long venueGroupId);
		VenueInfo GetVenue(long venueId);
		bool AddVenue(VenueInfo venue);
		bool UpdateVenue(VenueInfo venue);
		bool DeleteVenue(long venueId);
		bool SuspendVenue(long venueId, bool suspend);

		IEnumerable<VenueGroupInfo> GetVenueGroupsByGroup(long groupId, int postType);
		VenueGroupInfo GetVenueGroup(long venueGroupId);
		bool AddVenueGroup(VenueGroupInfo venueGroup);
		bool UpdateVenueGroup(VenueGroupInfo venueGroup);
		bool DeleteVenueGroup(long venueGroupId);

		VenueConfigInfo GetVenueConfig(long venueConfigId);
		void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs);
		BookingConfig GetVenueConfigs(long venueId);

		#endregion

		#region Post

		PostInfo GetPost(long postId, bool fromCache);
		bool AddPost(PostInfo post);
		bool UpdatePost(PostInfo post);
		bool DeletePost(long postId);

		bool AuditPost(long postId);
		bool ConcernPost(long postId, long userId);
		bool UnConcernPost(long postId, long userId);
		bool IsUserConcernPost(long postId, long userId);

		IEnumerable<PostInfo> GetPostsByType(bool fromCache, PostType type, bool withinTime, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByGroup(bool fromCache, long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByGroupTree(long groupId, UserInfo manager, string keyword, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByGroupTreeUnAudited(long groupId, UserInfo manager, string keyword, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetLastPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetLastHealthPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber);

		IEnumerable<PostInfo> GetPostsByGroupsUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByUserBooking(long userId, int pageIndex, int pageSize, out int totalNumber);

		#endregion

		#region Booking

		IList<BookingSchedule> GetBookingSchedules(long userId, long postId, long venueId, int days);
		IEnumerable<BookingInfo> GetBookingList(long postId, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<BookingInfo> GetBookingListByUserCode(string userCode, UserInfo manager);
		bool Booking(BookingInfo booking, out string message);
		bool CancelBooking(long userId, long postId, long venueConfigId, DateTime bookingDate);
		bool IsBookedPost(long userId, long postId);
		bool SigninBooking(long bookingId);
		bool SigninBooking(long managerId, string userCode);

        bool ExportBookingInfo();

		#endregion

		#region Message

		bool SendMessageToPostBookers(long postId, string title, string content, long senderId);
		bool SendMessageToGroupMembers(long groupId, string title, string content, long senderId);
		int GetUnreadMessagesCount(long userId);
		IEnumerable<MessageInfo> GetUserMessages(long userId, int pageIndex, int pageSize, out int totalNumber);
		bool ReadMessage(long userId, long messageId);
		bool DeleteMessage(long userId, long messageId);

		#endregion
	}
}
