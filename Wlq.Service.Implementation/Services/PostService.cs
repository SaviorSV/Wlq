using System;
using System.Collections.Generic;
using System.Linq;

using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Domain;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class PostService : ServiceBase, IPostService
	{
		[InjectionConstructor]
		public PostService(DatabaseContext databaseContext)
			: base(databaseContext)
		{ }

		#region venue

		public IEnumerable<VenueInfo> GetVenuesByGroup(long groupId)
		{
			return base.RepositoryProvider<VenueInfo>().Entities
				.Where(v => v.GroupId == groupId);
		}

		public VenueInfo GetVenue(long venueId)
		{
			return base.RepositoryProvider<VenueInfo>().GetById(venueId);
		}

		public bool AddVenue(VenueInfo venue)
		{
			return base.RepositoryProvider<VenueInfo>().Add(venue, true) > 0;
		}

		public bool UpdateVenue(VenueInfo venue)
		{
			return base.RepositoryProvider<VenueInfo>().Update(venue, true) > 0;
		}

		public bool DeleteVenue(long venueId)
		{
			base.RepositoryProvider<VenueInfo>().DeleteById(venueId, false);

			var venueConfigRepository = base.RepositoryProvider<VenueConfigInfo>();
			var venueConfigsGroups = venueConfigRepository.Entities
				.Where(c => c.VenueId == venueId);

			foreach (var venueConfig in venueConfigsGroups)
			{
				venueConfigRepository.Delete(venueConfig, false);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		public void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs)
		{
			var venueConfigRepository = base.RepositoryProvider<VenueConfigInfo>();
			var oldConfig = venueConfigRepository.Entities
				.Where(v => v.VenueId == venue.Id).ToList();

			//traverse sunday to saturday
			foreach (var day in configs.Keys)
			{
				if (configs[day] != null)
				{
					//visit each period in the day
					foreach (var period in configs[day])
					{
						var old = oldConfig.FirstOrDefault(c =>
							c.DaysOfWeek == (int)day &&
							c.BegenTime == period.BeginTime &&
							c.EndTime == period.EndTime &&
							c.LimitNumber == period.LimitNumber);

						//if the same period is not in the database, insert item
						if (old == null)
						{
							var newConfig = new VenueConfigInfo
							{
								VenueId = venue.Id,
								DaysOfWeek = (int)day,
								BegenTime = period.BeginTime,
								EndTime = period.EndTime,
								LimitNumber = period.LimitNumber
							};

							venueConfigRepository.Add(newConfig, false);
						}
						//remove the existing item from list
						else
						{
							oldConfig.Remove(old);
						}
					}
				}
			}

			//delete the rest items in list from database
			foreach (var restOld in oldConfig)
			{
				venueConfigRepository.Delete(restOld, false);
			}

			_databaseContext.SaveChanges();
		}

		public BookingConfig GetVenueConfigs(long venueId)
		{
			var configs = base.RepositoryProvider<VenueConfigInfo>().Entities
				.Where(v => v.VenueId == venueId);

			var bookingConfig = new BookingConfig();

			foreach (var config in configs)
			{
				try
				{
					bookingConfig.AddPeriod((DayOfWeek)config.DaysOfWeek, 
						config.BegenTime, config.EndTime, config.LimitNumber, config.Id);
				}
				catch (Exception ex)
				{
					LocalLoggingService.Exception(ex.Message);
					continue;
				}
			}

			return bookingConfig;
		}

		#endregion

		#region post

		public string GetPostTypeName(PostType type)
		{
			switch (type)
			{
				case PostType.All:
					return "全部";
				case PostType.Activity:
					return "活动";
				case PostType.Course:
					return "课程";
				case PostType.Venue:
					return "场地";
				case PostType.Health:
					return "健康";
				default:
					return "其他";
			}
		}

		public IEnumerable<PostInfo> GetPostsByType(PostType type, bool withinTime, int pageIndex, int pageSize, out int totalNumber)
		{
			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => (type == PostType.All || p.PostType == (int)type)
					&& (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByGroup(long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber)
		{
			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => p.GroupId == groupId
					&& (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetLastPosts(int pageIndex, int pageSize, out int totalNumber)
		{
			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByGroupsUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var groupIds = base.RepositoryProvider<UserGroupInfo>().Entities
				.Where(ug => ug.UserId == userId)
				.Select(ug => ug.GroupId);

			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && groupIds.Contains(p.GroupId))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByUser(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var postIds = base.RepositoryProvider<UserPostInfo>().Entities
				.Where(up => up.UserId == userId)
				.Select(up => up.PostId);

			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && postIds.Contains(p.Id))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByUserBooking(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var bookingPostIds = base.RepositoryProvider<BookingInfo>().Entities
				.Where(b => b.UserId == userId && b.VenueConfigId > 0 ? b.BookingDate > DateTime.Now.Date : true)
				.Select(b => b.PostId);

			return base.RepositoryProvider<PostInfo>().Entities
				.Where(p => DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && bookingPostIds.Contains(p.Id))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public PostInfo GetPost(long postId)
		{
			return base.RepositoryProvider<PostInfo>().GetById(postId);
		}

		public bool AddPost(PostInfo post)
		{
			return base.RepositoryProvider<PostInfo>().Add(post, true) > 0;
		}

		public bool UpdatePost(PostInfo post)
		{
			return base.RepositoryProvider<PostInfo>().Update(post, true) > 0;
		}

		public bool DeletePost(long postId)
		{
			return base.RepositoryProvider<PostInfo>().DeleteById(postId, true) > 0;
		}

		public bool ConcernPost(long postId, long userId)
		{
			if (IsUserConcernPost(postId, userId))
			{
				return true;
			}

			var userPost = new UserPostInfo
			{
				PostId = postId,
				UserId = userId
			};

			return base.RepositoryProvider<UserPostInfo>().Add(userPost, true) > 0;
		}

		public bool UnConcernPost(long postId, long userId)
		{
			var userPost = base.RepositoryProvider<UserPostInfo>().Entities
				.FirstOrDefault(up => up.PostId == postId && up.UserId == userId);

			if (userPost == null)
			{
				return true;
			}

			return base.RepositoryProvider<UserPostInfo>().Delete(userPost, true) > 0;
		}

		public bool IsUserConcernPost(long postId, long userId)
		{
			var userPost = base.RepositoryProvider<UserPostInfo>().Entities
				.FirstOrDefault(up => up.PostId == postId && up.UserId == userId);

			return userPost != null;
		}

		#endregion

		#region booking

		public bool Booking(BookingInfo booking, out string message)
		{
			message = "预订成功";

			var postRepository = base.RepositoryProvider<PostInfo>();
			var post = postRepository.GetById(booking.PostId);

			if (post == null || (DateTime.Now < post.BeginDate || DateTime.Now > post.EndDate))
			{
				message = "预订失败(发布信息失效)";
				return false;
			}

			if (post.PostType != (int)PostType.Venue && post.BookingNumber >= post.LimitNumber)
			{
				message = "预订失败(预定人数已满)";
				return false;
			}

			if (booking.VenueConfigId > 0)
			{
				var venueConfig = base.RepositoryProvider<VenueConfigInfo>().GetById(booking.VenueConfigId);

				if (venueConfig == null)
				{
					message = "预订失败(场馆配置不存在)";
					return false;
				}

				var venueBookingNumber = this.GetVenueBookingNumber(booking.PostId, booking.VenueConfigId, booking.BookingDate);

				if (venueBookingNumber >= venueConfig.LimitNumber)
				{
					message = "预订失败(场馆预定达到人数上限)";
					return false;
				}
			}

			base.RepositoryProvider<BookingInfo>().Add(booking, false);

			post.BookingNumber++;
			postRepository.Update(post, false);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool CancelBooking(long userId, long postId, long venueConfigId, DateTime bookingDate)
		{
			var bookingRepository = base.RepositoryProvider<BookingInfo>();
			var postRepository = base.RepositoryProvider<PostInfo>();

			var bookings = venueConfigId > 0
				? bookingRepository.Entities
					.Where(b => b.UserId == userId && b.PostId == postId && b.VenueConfigId == venueConfigId && b.BookingDate.Date == bookingDate.Date)
				: bookingRepository.Entities
					.Where(b => b.UserId == userId && b.PostId == postId);

			foreach (var booking in bookings)
			{
				bookingRepository.Delete(booking, false);
			}

			var post = postRepository.GetById(postId);

			if (post != null && post.BookingNumber > 0)
			{
				post.BookingNumber--;
				postRepository.Update(post, false);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		private int GetVenueBookingNumber(long postId, long venueConfigId, DateTime bookingDate)
		{
			var bookings = base.RepositoryProvider<BookingInfo>().Entities
				.Where(b => b.PostId == postId && b.VenueConfigId == venueConfigId && b.BookingDate.Date == bookingDate.Date);

			return bookings.Count();
		}

		public List<BookingSchedule> GetBookingSchedules(long userId, long postId, int days)
		{
			var post = base.RepositoryProvider<PostInfo>().GetById(postId);

			if (post == null || post.VenueId == 0)
			{
				return null;
			}

			var venueConfigs = this.GetVenueConfigs(post.VenueId);
			var schedules = new List<BookingSchedule>();
			var today = DateTime.Now.Date;

			for (int i = 1; i <= days; i++)
			{
				var date = today.AddDays(i);

				var schedule = new BookingSchedule
				{
					Date = date,
					Periods = venueConfigs[date.DayOfWeek]
				};

				foreach (var period in schedule.Periods)
				{
					period.BookingNumber = this.GetVenueBookingNumber(postId, period.VenueConfigId, date);

					if (period.BookingNumber > 0)
					{
						period.IsBooked = this.IsBookedVenue(postId, userId, period.VenueConfigId, date);
					}
				}

				schedules.Add(schedule);
			}

			return schedules;
		}

		private bool IsBookedVenue(long postId, long userId, long venueConfigId, DateTime bookingDate)
		{
			var booking = base.RepositoryProvider<BookingInfo>().Entities
				.FirstOrDefault(b => b.PostId == postId && b.UserId == userId && b.VenueConfigId == venueConfigId && b.BookingDate.Date == bookingDate.Date);

			return booking != null;
		}

		public bool IsBookedPost(long postId, long userId)
		{
			var booking = base.RepositoryProvider<BookingInfo>().Entities
				.FirstOrDefault(b => b.PostId == postId && b.UserId == userId);

			return booking != null;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
