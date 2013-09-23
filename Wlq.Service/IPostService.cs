using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IPostService : IDisposable
	{
		#region Venue

		string GetVenueTypeName(VenueType type);
		IEnumerable<VenueInfo> GetVenuesByGroup(long groupId);
		IEnumerable<VenueInfo> GetVenuesByVenueGroup(long venueGroupId);
		VenueInfo GetVenue(long venueId);
		bool AddVenue(VenueInfo venue);
		bool UpdateVenue(VenueInfo venue);
		bool DeleteVenue(long venueId);

		IEnumerable<VenueGroupInfo> GetVenueGroupsByGroup(long groupId);
		VenueGroupInfo GetVenueGroup(long venueGroupId);
		bool AddVenueGroup(VenueGroupInfo venueGroup);
		bool UpdateVenueGroup(VenueGroupInfo venueGroup);
		bool DeleteVenueGroup(long venueGroupId);

		void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs);
		BookingConfig GetVenueConfigs(long venueId);

		#endregion

		#region Post

		string GetPostTypeName(PostType type);

		PostInfo GetPost(long postId, bool fromCache);
		bool AddPost(PostInfo post);
		bool UpdatePost(PostInfo post);
		bool DeletePost(long postId);

		bool ConcernPost(long postId, long userId);
		bool UnConcernPost(long postId, long userId);
		bool IsUserConcernPost(long postId, long userId);

		IEnumerable<PostInfo> GetPostsByType(bool fromCache, PostType type, bool withinTime, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByGroup(bool fromCache, long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetLastPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetLastHealthPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber);

		IEnumerable<PostInfo> GetPostsByGroupsUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByUserBooking(long userId, int pageIndex, int pageSize, out int totalNumber);

		#endregion

		#region Booking

		List<BookingSchedule> GetBookingSchedules(long userId, long postId, long venueId, int days);
		bool Booking(BookingInfo booking, out string message);
		bool CancelBooking(long userId, long postId, long venueConfigId, DateTime bookingDate);
		bool IsBookedPost(long postId, long userId);

		#endregion

		#region Message

		void SendMessageToPostBookers(long postId, string title, string content, long senderId);
		int GetUnreadMessagesCount(long userId);
		IEnumerable<MessageInfo> GetUserMessages(long userId, int pageIndex, int pageSize, out int totalNumber);
		void ReadMessage(long userId, long messageId);

		#endregion
	}
}
