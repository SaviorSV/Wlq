﻿using System;
using System.Collections.Generic;
using System.Linq;

using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Domain;
using Wlq.Persistence;

namespace Wlq.Service.Implementation
{
	public class PostService : Disposable, IPostService
	{
		private readonly DatabaseContext _databaseContext;

		[InjectionConstructor]
		public PostService(DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		#region venue

		public IEnumerable<VenueInfo> GetVenuesByGroup(long groupId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			return venueRepository.GetAll()
				.Where(v => v.GroupId == groupId);
		}

		public VenueInfo GetVenue(long venueId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			return venueRepository.GetById(venueId);
		}

		public bool AddVenue(VenueInfo venue)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			venueRepository.Add(venue);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool UpdateVenue(VenueInfo venue)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);

			venueRepository.Update(venue);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool DeleteVenue(long venueId)
		{
			var venueRepository = new DatabaseRepository<VenueInfo>(_databaseContext);
			var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);

			venueRepository.DeleteById(venueId);

			var venueConfigsGroups = venueConfigRepository.GetAll()
				.Where(c => c.VenueId == venueId);

			foreach (var venueConfig in venueConfigsGroups)
			{
				venueConfigRepository.DeleteById(venueConfig.Id);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		public void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs)
		{
			var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);
			var oldConfig = venueConfigRepository.GetAll().Where(v => v.VenueId == venue.Id).ToList();

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

							venueConfigRepository.Add(newConfig);
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
				venueConfigRepository.DeleteById(restOld.Id);
			}

			_databaseContext.SaveChanges();
		}

		public BookingConfig GetVenueConfigs(long venueId)
		{
			var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);
			var configs = venueConfigRepository.GetAll().Where(v => v.VenueId == venueId);
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
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			return postRepository.GetAll()
				.Where(p => (type == PostType.All || p.PostType == (int)type)
					&& (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByGroup(long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			return postRepository.GetAll()
				.Where(p => p.GroupId == groupId
					&& (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
				.OrderByDescending(p => p.LastModified)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public PostInfo GetPost(long postId)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			return postRepository.GetById(postId);
		}

		public bool AddPost(PostInfo post)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			postRepository.Add(post);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool UpdatePost(PostInfo post)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			postRepository.Update(post);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool DeletePost(long postId)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);

			postRepository.DeleteById(postId);

			return _databaseContext.SaveChanges() > 0;
		}

		#endregion

		#region booking

		public bool Booking(BookingInfo booking, out string message)
		{
			message = "预订成功";

			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);
			var post = postRepository.GetById(booking.PostId);

			if (post == null || (DateTime.Now < post.BeginDate || DateTime.Now > post.EndDate))
			{
				message = "预订失败(发布信息失效)";
				return false;
			}

			if (booking.VenueConfigId > 0)
			{
				var venueConfigRepository = new DatabaseRepository<VenueConfigInfo>(_databaseContext);
				var venueConfig = venueConfigRepository.GetById(booking.VenueConfigId);

				if (venueConfig == null)
				{
					message = "预订失败(场馆配置不存在)";
					return false;
				}

				var venueBookingNumber = this.GetVenueBooingNumber(booking.PostId, booking.VenueConfigId, booking.BookingDate);

				if (venueBookingNumber >= venueConfig.LimitNumber)
				{
					message = "预订失败(场馆预定达到人数上限)";
					return false;
				}
			}

			var bookingRepository = new DatabaseRepository<BookingInfo>(_databaseContext);
			bookingRepository.Add(booking);

			post.BookingNumber++;
			postRepository.Update(post);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool CancelBooking(long userId, long postId)
		{
			var bookingRepository = new DatabaseRepository<BookingInfo>(_databaseContext);

			var bookings = bookingRepository.GetAll()
				.Where(b => b.UserId == userId && b.PostId == postId);

			foreach (var booking in bookings)
			{
				bookingRepository.DeleteById(booking.Id);
			}

			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);
			var post = postRepository.GetById(postId);

			if (post != null && post.BookingNumber > 0)
			{
				post.BookingNumber--;
				postRepository.Update(post);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		private int GetVenueBooingNumber(long postId, long venueConfigId, DateTime bookingDate)
		{
			var bookingRepository = new DatabaseRepository<BookingInfo>(_databaseContext);

			var bookings = bookingRepository.GetAll()
				.Where(b => b.PostId == postId && b.VenueConfigId == venueConfigId && b.BookingDate.Date == bookingDate.Date);

			return bookings.Count();
		}

		public List<BookingSchedule> GetBookingSchedules(long postId, int days)
		{
			var postRepository = new DatabaseRepository<PostInfo>(_databaseContext);
			var post = postRepository.GetById(postId);

			if (post == null || post.VenueId == 0)
			{
				return null;
			}

			var venueConfigs = this.GetVenueConfigs(post.VenueId);
			var schedules = new List<BookingSchedule>();

			for (int i = 0; i < days; i++)
			{
				var date = DateTime.Now.AddDays(i).Date;

				var schedule = new BookingSchedule
				{
					Date = date,
					Periods = venueConfigs[date.DayOfWeek]
				};

				foreach (var period in schedule.Periods)
				{
					period.BookingNumber = this.GetVenueBooingNumber(postId, period.VenueConfigId, date);
				}

				schedules.Add(schedule);
			}

			return schedules;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
