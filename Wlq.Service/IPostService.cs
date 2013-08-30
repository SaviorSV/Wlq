using System;
using System.Collections.Generic;

using Wlq.Domain;

namespace Wlq.Service
{
	public interface IPostService : IDisposable
	{
		#region Venue

		IEnumerable<VenueInfo> GetVenuesByGroup(long groupId);
		VenueInfo GetVenue(long venueId);
		bool AddVenue(VenueInfo venue);
		bool UpdateVenue(VenueInfo venue);
		bool DeleteVenue(long venueId);

		void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs);
		BookingConfig GetVenueConfigs(long venueId);

		#endregion

		#region Post

		string GetPostTypeName(PostType type);

		PostInfo GetPost(long postId);
		bool AddPost(PostInfo post);
		bool UpdatePost(PostInfo post);
		bool DeletePost(long postId);

		bool ConcernPost(long postId, long userId);
		bool UnConcernPost(long postId, long userId);
		bool IsUserConcernPost(long postId, long userId);

		IEnumerable<PostInfo> GetPostsByType(PostType type, bool withinTime, int pageIndex, int pageSize, out int totalNumber);
		IEnumerable<PostInfo> GetPostsByGroup(long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber);

		#endregion

		#region booking

		List<BookingSchedule> GetBookingSchedules(long postId, int days);
		bool Booking(BookingInfo booking, out string message);
		bool CancelBooking(long userId, long postId);

		#endregion
	}
}
