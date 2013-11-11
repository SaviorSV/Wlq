using System;
using System.Collections.Generic;
using System.Linq;

using Hanger.Caching;
using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Domain;
using Wlq.Persistence;
using System.IO;
using OfficeOpenXml;

namespace Wlq.Service.Implementation
{
	public class PostService : ServiceBase, IPostService
	{
		private static readonly TimeSpan _PostListCachedTime = new TimeSpan(0, 5, 0);

		[InjectionConstructor]
		public PostService(DatabaseContext databaseContext)
			: base(databaseContext)
		{ }

		#region venue

		public IEnumerable<VenueGroupInfo> GetVenueGroupsByGroup(long groupId, int postType)
		{
			var venueGroups = base.GetRepository<VenueGroupInfo>().Entities
				.Where(v => v.GroupId == groupId);

			if (postType != (int)PostType.All)
			{
				venueGroups = venueGroups.Where(v => v.PostType == postType);
			}

			return venueGroups;
		}

		public VenueGroupInfo GetVenueGroup(long venueGroupId)
		{
			return base.GetRepository<VenueGroupInfo>().GetById(venueGroupId);
		}

		public bool AddVenueGroup(VenueGroupInfo venueGroup)
		{
			return base.GetRepository<VenueGroupInfo>().Add(venueGroup, true) > 0;
		}

		public bool UpdateVenueGroup(VenueGroupInfo venueGroup)
		{
			return base.GetRepository<VenueGroupInfo>().Update(venueGroup, true) > 0;
		}

		public bool DeleteVenueGroup(long venueGroupId)
		{
			var venues = base.GetRepository<VenueInfo>().Entities
				.Where(v => v.VenueGroupId == venueGroupId);

			foreach (var venue in venues)
			{
				this.DeleteVenue(venue.Id);
			}

			return base.GetRepository<VenueGroupInfo>().DeleteById(venueGroupId, true) > 0; 
		}

		public bool SuspendVenue(long venueId, bool suspend)
		{
			var repository = base.GetRepository<VenueInfo>();
			var venue = repository.GetById(venueId);

			if (venue != null)
			{
				venue.IsSuspend = suspend;

				return repository.Update(venue, true) > 0;
			}

			return false;
		}

		public IEnumerable<VenueInfo> GetVenuesByVenueGroup(long venueGroupId)
		{
			return base.GetRepository<VenueInfo>().Entities
				.Where(v => v.VenueGroupId == venueGroupId);
		}

		public IEnumerable<VenueInfo> GetVenuesByVenueGroupNotSuspended(long venueGroupId)
		{
			return base.GetRepository<VenueInfo>().Entities
				.Where(v => v.VenueGroupId == venueGroupId && v.IsSuspend == false);
		}

		public VenueInfo GetVenue(long venueId)
		{
			return base.GetRepository<VenueInfo>().GetById(venueId);
		}

		public bool AddVenue(VenueInfo venue)
		{
			return base.GetRepository<VenueInfo>().Add(venue, true) > 0;
		}

		public bool UpdateVenue(VenueInfo venue)
		{
			return base.GetRepository<VenueInfo>().Update(venue, true) > 0;
		}

		public bool DeleteVenue(long venueId)
		{
			base.GetRepository<VenueInfo>().DeleteById(venueId, false);

			var venueConfigRepository = base.GetRepository<VenueConfigInfo>();
			var venueConfigsGroups = venueConfigRepository.Entities
				.Where(c => c.VenueId == venueId);

			foreach (var venueConfig in venueConfigsGroups)
			{
				venueConfigRepository.Delete(venueConfig, false);
			}

			return _databaseContext.SaveChanges() > 0;
		}

		public VenueConfigInfo GetVenueConfig(long venueConfigId)
		{
			return base.GetRepository<VenueConfigInfo>().GetById(venueConfigId);
		}

		public void SaveVenueConfigs(VenueInfo venue, Dictionary<DayOfWeek, List<BookingPeriod>> configs)
		{
			var venueConfigRepository = base.GetRepository<VenueConfigInfo>();
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
			var configs = base.GetRepository<VenueConfigInfo>().Entities
				.Where(v => v.VenueId == venueId)
				.OrderBy(v => v.BegenTime);

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

		public IEnumerable<PostInfo> GetPostsByType(bool fromCache, PostType type, bool withinTime, int pageIndex, int pageSize, out int totalNumber)
		{
			var key = string.Format("PostService.GetPostsByType.{0}.{1}"
				, (int)type, withinTime);

			var postList = CacheManager.GetList<PostInfo>(fromCache, key, pageIndex, pageSize, _PostListCachedTime,
			() =>
			{
				return base.GetRepository<PostInfo>().Entities
					.Where(p => p.IsAudited && (type == PostType.All || p.PostType == (int)type) && (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
					.OrderByDescending(p => p.PublishTime);
			});
		
			totalNumber = postList.TotalNumber;

			return postList.List;
		}

		public IEnumerable<PostInfo> GetPostsByGroup(bool fromCache, long groupId, bool withinTime, int pageIndex, int pageSize, out int totalNumber)
		{
			var key = string.Format("PostService.GetPostsByGroup.{0}.{1}"
				, groupId, withinTime);

			var postList = CacheManager.GetList<PostInfo>(fromCache, key, pageIndex, pageSize, _PostListCachedTime,
			() =>
			{
				return base.GetRepository<PostInfo>().Entities
					.Where(p => p.IsAudited && p.GroupId == groupId && (!withinTime || (DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)))
					.OrderByDescending(p => p.PublishTime);
			});

			totalNumber = postList.TotalNumber;

			return postList.List;
		}

		public IEnumerable<PostInfo> GetPostsByGroupTree(long groupId, UserInfo manager, string keyword, int pageIndex, int pageSize, out int totalNumber)
		{
			var posts = base.GetRepository<PostInfo>().Entities;

			if (groupId > 0)
			{
				var groupIds = base.GetRepository<GroupInfo>().Entities
					.Where(g => g.ParentGroupId == groupId)
					.Select(g => g.Id);

				posts = posts.Where(p => p.GroupId == groupId || groupIds.Contains(p.GroupId));
			}
			else if(manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var parentGroupIds = base.GetRepository<GroupManagerInfo>().Entities
					.Where(r => r.UserId == manager.Id)
					.Select(r => r.GroupId);

				var groupIds = base.GetRepository<GroupInfo>().Entities
					.Where(g => parentGroupIds.Contains(g.ParentGroupId))
					.Select(g => g.Id);

				posts = posts.Where(p => parentGroupIds.Contains(p.GroupId) || groupIds.Contains(p.GroupId));
			}

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				posts = posts.Where(p => p.Title.Contains(keyword));
			}

			return posts
				.OrderByDescending(p => p.PublishTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByGroupTreeUnAudited(long groupId, UserInfo manager, string keyword, int pageIndex, int pageSize, out int totalNumber)
		{
			var posts = base.GetRepository<PostInfo>().Entities
				.Where(p => p.IsAudited == false);

			if (groupId > 0)
			{
				var groupIds = base.GetRepository<GroupInfo>().Entities
					.Where(g => g.ParentGroupId == groupId)
					.Select(g => g.Id);

				posts = posts.Where(p => groupIds.Contains(p.GroupId));
			}
			else if (manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var parentGroupIds = base.GetRepository<GroupManagerInfo>().Entities
					.Where(r => r.UserId == manager.Id)
					.Select(r => r.GroupId);

				var groupIds = base.GetRepository<GroupInfo>().Entities
					.Where(g => parentGroupIds.Contains(g.ParentGroupId))
					.Select(g => g.Id);

				posts = posts.Where(p => groupIds.Contains(p.GroupId));
			}

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				posts = posts.Where(p => p.Title.Contains(keyword));
			}

			return posts
				.OrderByDescending(p => p.PublishTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetLastPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber)
		{
			var key = "PostService.GetLastPosts";

			var postList = CacheManager.GetList<PostInfo>(fromCache, key, pageIndex, pageSize, _PostListCachedTime,
			() =>
			{
				return base.GetRepository<PostInfo>().Entities
					.Where(p => p.IsAudited && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate)
					.OrderByDescending(p => p.PublishTime);
			});

			totalNumber = postList.TotalNumber;

			return postList.List;
		}

		public IEnumerable<PostInfo> GetLastHealthPosts(bool fromCache, int pageIndex, int pageSize, out int totalNumber)
		{
			var key = "PostService.GetLastHealthPosts";

			var postList = CacheManager.GetList<PostInfo>(fromCache, key, pageIndex, pageSize, _PostListCachedTime,
			() =>
			{
				return base.GetRepository<PostInfo>().Entities
					.Where(p => p.IsAudited && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && p.IsHealthTopic)
					.OrderByDescending(p => p.PublishTime);
			});

			totalNumber = postList.TotalNumber;

			return postList.List;
		}

		public IEnumerable<PostInfo> GetPostsByGroupsUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var groupIds = base.GetRepository<UserGroupInfo>().Entities
				.Where(ug => ug.UserId == userId)
				.Select(ug => ug.GroupId);

			return base.GetRepository<PostInfo>().Entities
				.Where(p => p.IsAudited && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && groupIds.Contains(p.GroupId))
				.OrderByDescending(p => p.PublishTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByUserConcerned(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var postIds = base.GetRepository<UserPostInfo>().Entities
				.Where(up => up.UserId == userId)
				.Select(up => up.PostId);

			return base.GetRepository<PostInfo>().Entities
				.Where(p => p.IsAudited && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && postIds.Contains(p.Id))
				.OrderByDescending(p => p.PublishTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<PostInfo> GetPostsByUserBooking(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var bookingPostIds = base.GetRepository<BookingInfo>().Entities
				.Where(b => b.UserId == userId && (b.VenueConfigId == 0 || b.BookingDate >= DateTime.Today))
				.Select(b => b.PostId)
				.Distinct();

			return base.GetRepository<PostInfo>().Entities
				.Where(p => p.IsAudited && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate && bookingPostIds.Contains(p.Id))
				.OrderByDescending(p => p.PublishTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public PostInfo GetPost(long postId, bool fromCache)
		{
			var key = string.Format("Wlq.Domain.PostInfo.{0}", postId);

			return CacheManager.Get<PostInfo>(fromCache, key, new TimeSpan(0, 2, 0),
				() =>
				{
					return base.GetRepository<PostInfo>().GetById(postId); ;
				});
		}

		public bool AddPost(PostInfo post)
		{
			return base.GetRepository<PostInfo>().Add(post, true) > 0;
		}

		public bool UpdatePost(PostInfo post)
		{
			var success = base.GetRepository<PostInfo>().Update(post, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.PostInfo.{0}", post.Id);

				CacheManager.Update<PostInfo>(key, post, new TimeSpan(0, 3, 0));
			}

			return success;
		}

		public bool DeletePost(long postId)
		{
			var success = base.GetRepository<PostInfo>().DeleteById(postId, true) > 0;

			if (success)
			{
				var key = string.Format("Wlq.Domain.PostInfo.{0}", postId);

				CacheManager.RemoveKey(key);
			}

			return success;
		}

		public bool AuditPost(long postId)
		{
			var repository = base.GetRepository<PostInfo>();
			var post = repository.GetById(postId);

			if (post != null)
			{
				post.IsAudited = true;

				return repository.Update(post, true) > 0;
			}

			return false;
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

			return base.GetRepository<UserPostInfo>().Add(userPost, true) > 0;
		}

		public bool UnConcernPost(long postId, long userId)
		{
			var userPost = base.GetRepository<UserPostInfo>().Entities
				.FirstOrDefault(up => up.PostId == postId && up.UserId == userId);

			if (userPost == null)
			{
				return true;
			}

			return base.GetRepository<UserPostInfo>().Delete(userPost, true) > 0;
		}

		public bool IsUserConcernPost(long postId, long userId)
		{
			var userPost = base.GetRepository<UserPostInfo>().Entities
				.FirstOrDefault(up => up.PostId == postId && up.UserId == userId);

			return userPost != null;
		}

		#endregion

		#region booking

		public bool Booking(BookingInfo booking, out string message)
		{
			message = "预订成功";

			var postRepository = base.GetRepository<PostInfo>();
			var post = postRepository.GetById(booking.PostId);

			if (post == null || (DateTime.Now < post.BeginDate || DateTime.Now > post.EndDate))
			{
				message = "预约失败(发布信息失效)";
				return false;
			}

			if (booking.VenueConfigId == 0 && post.BookingNumber >= post.LimitNumber)
			{
				message = "预约失败(预约人数已满)";
				return false;
			}

			var bookingRepository = base.GetRepository<BookingInfo>();

			if (booking.VenueConfigId > 0)
			{
				var venueConfig = base.GetRepository<VenueConfigInfo>().GetById(booking.VenueConfigId);

				if (venueConfig == null)
				{
					message = "预约失败(该配置不存在)";
					return false;
				}

				var booked = bookingRepository.Entities
					.FirstOrDefault(b => b.UserId == booking.UserId && b.PostId == booking.PostId && b.VenueConfigId == booking.VenueConfigId && b.BookingDate == booking.BookingDate.Date);

				if (booked != null)
				{
					message = "预约失败(已预约)";
					return false;
				}

				var venueBookingNumber = this.GetVenueBookingNumber(booking.PostId, booking.VenueConfigId, booking.BookingDate);

				if (venueBookingNumber >= venueConfig.LimitNumber)
				{
					message = "预约失败(预约达到人数上限)";
					return false;
				}
			}

			bookingRepository.Add(booking, false);

			post.BookingNumber++;
			postRepository.Update(post, false);

			return _databaseContext.SaveChanges() > 0;
		}

		public bool CancelBooking(long userId, long postId, long venueConfigId, DateTime bookingDate)
		{
			var bookingRepository = base.GetRepository<BookingInfo>();
			var postRepository = base.GetRepository<PostInfo>();

			var bookings = venueConfigId > 0
				? bookingRepository.Entities
					.Where(b => b.UserId == userId && b.PostId == postId && b.VenueConfigId == venueConfigId && b.BookingDate == bookingDate.Date)
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
			var bookings = base.GetRepository<BookingInfo>().Entities
				.Where(b => b.PostId == postId && b.VenueConfigId == venueConfigId && b.BookingDate == bookingDate.Date);

			return bookings.Count();
		}

		public IList<BookingSchedule> GetBookingSchedules(long userId, long postId, long venueId, int days)
		{
			var post = base.GetRepository<PostInfo>().GetById(postId);

			if (post == null)
			{
				return null;
			}

			var venue = base.GetRepository<VenueInfo>().GetById(venueId);

			if (venue == null)
			{
				return null;
			}

			var venueConfigs = this.GetVenueConfigs(venueId);
			var schedules = new List<BookingSchedule>();
			var today = DateTime.Now.Date;

			for (int i = 1; i <= days; i++)
			{
				var date = today.AddDays(i);

				var schedule = new BookingSchedule
				{
					Date = date.ToString("yyyy/MM/dd"),
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

		public IEnumerable<BookingInfo> GetBookingList(long postId, int pageIndex, int pageSize, out int totalNumber)
		{
			return base.GetRepository<BookingInfo>().Entities
				.Where(b => b.PostId == postId)
				.OrderByDescending(b => b.BookingDate)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public IEnumerable<BookingInfo> GetBookingListByUserCode(string userCode, UserInfo manager)
		{
			var user = base.GetRepository<UserInfo>().Entities
				.FirstOrDefault(u => u.Code == userCode);

			if (user == null)
			{
				return new List<BookingInfo>();
			}

			var posts = base.GetRepository<PostInfo>().Entities
				.Where(p => p.IsAudited == true && DateTime.Now >= p.BeginDate && DateTime.Now <= p.EndDate);

			if (manager.Role != (int)RoleLevel.SuperAdmin)
			{
				var parentGroupIds = base.GetRepository<GroupManagerInfo>().Entities
					.Where(r => r.UserId == manager.Id)
					.Select(r => r.GroupId);

				var groupIds = base.GetRepository<GroupInfo>().Entities
					.Where(g => parentGroupIds.Contains(g.ParentGroupId))
					.Select(g => g.Id);

				posts = posts.Where(p => parentGroupIds.Contains(p.GroupId) || groupIds.Contains(p.GroupId));
			}

			return base.GetRepository<BookingInfo>().Entities
				.Where(b => b.UserId == user.Id && posts.Select(p => p.Id).Contains(b.PostId) && (b.VenueConfigId == 0 || b.BookingDate >= DateTime.Today))
				.OrderByDescending(b => b.BookingDate);
		}

		private bool IsBookedVenue(long postId, long userId, long venueConfigId, DateTime bookingDate)
		{
			var booking = base.GetRepository<BookingInfo>().Entities
				.FirstOrDefault(b => b.PostId == postId && b.UserId == userId && b.VenueConfigId == venueConfigId && b.BookingDate == bookingDate.Date);

			return booking != null;
		}

		public bool IsBookedPost(long userId, long postId)
		{
			var booking = base.GetRepository<BookingInfo>().Entities
				.FirstOrDefault(b => b.PostId == postId && b.UserId == userId);

			return booking != null;
		}

		public bool SigninBooking(long bookingId)
		{
			var bookingRepository = base.GetRepository<BookingInfo>();
			var booking = bookingRepository.GetById(bookingId);

			if (booking != null)
			{
				booking.IsPresent = true;
				booking.PresentTime = DateTime.Now;

				return bookingRepository.Update(booking, true) > 0;
			}

			return false;
		}

		public bool SigninBooking(long managerId, string userCode)
		{
			var userRepository= base.GetRepository<UserInfo>();
			var user = userRepository.Entities.FirstOrDefault(u => u.Code == userCode);

			if (user == null)
			{
				return false;
			}

			var manager = userRepository.GetById(managerId);

			if (manager == null)
			{
				return false;
			}

			var bookingRepository = base.GetRepository<BookingInfo>();
			var bookingList = bookingRepository.Entities
				.Where(b => b.UserId == user.Id && b.BookingDate == DateTime.Today);

			var find = false;

			if (bookingList.Count() > 0)
			{
				foreach (var bookingInfo in bookingList)
				{
					if (manager.Role == (int)RoleLevel.SuperAdmin || this.PostIsManagedByManager(bookingInfo.PostId, managerId))
					{
						find = true;

						if (!bookingInfo.IsPresent)
						{
							bookingInfo.IsPresent = true;
							bookingInfo.PresentTime = DateTime.Now;

							bookingRepository.Update(bookingInfo, false);
						}
					}
				}

				_databaseContext.SaveChanges();
			}

			return find;
		}

		private bool PostIsManagedByManager(long postId, long managerId)
		{
			var post = base.GetRepository<PostInfo>().GetById(postId);

			if (post == null)
			{
				return false;
			}

			var relation = base.GetRepository<GroupManagerInfo>().Entities
				.FirstOrDefault(r => r.GroupId == post.GroupId && r.UserId == managerId);

			return relation != null;
		}

        public bool ExportBookingInfo()
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Temp\\ExportBookingInfo.xls";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var newFile = new FileInfo(filePath);

            using (var pck = new ExcelPackage(newFile))
            {
                var ws = pck.Workbook.Worksheets.Add("Sheet1");
                //ws.View.ShowGridLines = false;

                ws.Cells["A1"].Value = "卡号";
                ws.Cells["B1"].Value = "姓名";
                ws.Cells["C1"].Value = "预约信息";
                ws.Cells["D1"].Value = "签到信息";

                for (int i = 2; i <= 12; i++)
                {
                    ws.Cells["A" + i.ToString()].Value = "";
                }

                pck.SaveAs(newFile);
            }

            return true;
        }

		#endregion

		#region message

		public bool SendMessageToPostBookers(long postId, string title, string content, long senderId)
		{
			var post = base.GetRepository<PostInfo>().GetById(postId);

			if (post == null)
			{
				return false;
			}

			var message = new MessageInfo
			{
				Title = title,
				Content = content,
				PostId = postId,
				SenderId = senderId
			};

			base.GetRepository<MessageInfo>().Add(message, true);

			var bookerIds = base.GetRepository<BookingInfo>().Entities
				.Where(b => b.PostId == postId && (b.VenueConfigId == 0 || b.BookingDate > DateTime.Now))
				.Select(b => b.UserId)
				.Distinct();

			var userMassages = new List<UserMessageInfo>();

			foreach (var userId in bookerIds)
			{
				var userMassage = new UserMessageInfo
				{
					UserId = userId,
					MessageId = message.Id
				};

				userMassages.Add(userMassage);
			}

			if (userMassages.Count > 0)
			{
				return base.GetRepository<UserMessageInfo>().BatchInsert(userMassages);
			}

			return true;
		}

		public bool SendMessageToGroupMembers(long groupId, string title, string content, long senderId)
		{
			var group = base.GetRepository<GroupInfo>().GetById(groupId);

			if (group == null)
			{
				return false;
			}

			var message = new MessageInfo
			{
				Title = title,
				Content = content,
				PostId = 0,
				SenderId = senderId
			};

			base.GetRepository<MessageInfo>().Add(message, true);

			var userIds = base.GetRepository<UserGroupInfo>().Entities
				.Where(ug => ug.GroupId == group.Id)
				.Select(b => b.UserId)
				.Distinct();

			var userMassages = new List<UserMessageInfo>();

			foreach (var userId in userIds)
			{
				var userMassage = new UserMessageInfo
				{
					UserId = userId,
					MessageId = message.Id
				};

				userMassages.Add(userMassage);
			}

			if (userMassages.Count > 0)
			{
				return base.GetRepository<UserMessageInfo>().BatchInsert(userMassages);
			}

			return true;
		}

		public int GetUnreadMessagesCount(long userId)
		{
			return base.GetRepository<UserMessageInfo>().Entities
				.Where(um => um.UserId == userId && um.IsRead == false)
				.Count();
		}

		public IEnumerable<MessageInfo> GetUserMessages(long userId, int pageIndex, int pageSize, out int totalNumber)
		{
			var messageIds = base.GetRepository<UserMessageInfo>().Entities
				.Where(um => um.UserId == userId)
				.Select(um => um.MessageId);

			return base.GetRepository<MessageInfo>().Entities
				.Where(m => messageIds.Contains(m.Id))
				.OrderByDescending(m => m.SendTime)
				.Paging(pageIndex, pageSize, out totalNumber);
		}

		public bool ReadMessage(long userId, long messageId)
		{
			var userMessageRepository = base.GetRepository<UserMessageInfo>();
			var userMessage = userMessageRepository.Entities
				.FirstOrDefault(um => um.UserId == userId && um.MessageId == messageId);

			if (userMessage != null)
			{
				userMessage.IsRead = true;
				return userMessageRepository.Update(userMessage, true) > 0;
			}

			return false;
		}

		public bool DeleteMessage(long userId, long messageId)
		{
			var userMessageRepository = base.GetRepository<UserMessageInfo>();
			var userMessage = userMessageRepository.Entities
				.FirstOrDefault(um => um.UserId == userId && um.MessageId == messageId);

			if (userMessage != null)
			{
				return userMessageRepository.Delete(userMessage, true) > 0;
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		protected override void InternalDispose() { }
	}
}
