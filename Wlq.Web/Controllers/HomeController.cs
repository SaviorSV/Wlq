using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Web.Models;

namespace Wlq.Web.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index(int pageIndex = 1, PostType postType = PostType.All)
		{
			var totalNumber = 0;
			var posts = postType == PostType.All
				? PostService.GetLastPosts(pageIndex, _PostListSize, out totalNumber)
				: PostService.GetPostsByType(postType, true, pageIndex, _PostListSize, out totalNumber);

			ViewBag.PageIndex = pageIndex;
			ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;
			ViewBag.PostType = postType;
			ViewBag.CurrentUserId = CurrentUserId;

			var modelList = new List<PostModel>();

			foreach (var post in posts)
			{
				modelList.Add(new PostModel
				{
					Post = post,
					Group = UserGroupService.GetGroup(post.GroupId),
					IsBooked = CurrentUserId > 0 && post.PostType != (int)PostType.Venue
						? PostService.IsBookedPost(post.Id, CurrentUserId) : false
				});
			}

			return View("_List", modelList);
		}

		public ActionResult Health(int pageIndex = 1)
		{
			//todo: Health page
			return View();
		}

		public ActionResult TZJC()
		{
			return View();
		}

		public ActionResult JKDA()
		{
			return View();
		}

		public ActionResult JKBD()
		{
			return View();
		}

		public ActionResult Group(long id, int pageIndex = 1)
		{
			var group = UserGroupService.GetGroup(id);

			if (group == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var totalNumber = 0;
			var posts = PostService.GetPostsByGroup(id, true, pageIndex, _GroupListSize, out totalNumber);
			var postModels = new List<PostModel>();

			foreach (var post in posts)
			{
				postModels.Add(new PostModel
				{
					Post = post,
					Group = group,
					IsBooked = CurrentUserId > 0 && post.PostType != (int)PostType.Venue
						? PostService.IsBookedPost(post.Id, CurrentUserId) : false
				});
			}

			var model = new GroupDetailModel();

			model.Group = group;
			model.IsFollowing = CurrentUserId > 0
				? UserGroupService.IsUserInGroup(CurrentUserId, group.Id) : false;
			model.Posts = postModels;

			ViewBag.PageIndex = pageIndex;
			ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;
			ViewBag.CurrentUserId = CurrentUserId;
			
			return View(model);
		}

		public ActionResult GroupList(int pageIndex = 1)
		{
			var totalNumber = 0;
			var circles = UserGroupService.GetCircles(pageIndex, _GroupListSize, out totalNumber);

			ViewBag.PageIndex = pageIndex;
			ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _PostListSize) : 1;
			ViewBag.CurrentUserId = CurrentUserId;

			var modelList = new List<GroupModel>();

			foreach (var group in circles)
			{
				modelList.Add(new GroupModel
				{
					Group = group,
					IsFollowing = CurrentUserId > 0
						? UserGroupService.IsUserInGroup(CurrentUserId, group.Id) : false
				});
			}

			return View(modelList);
		}

		public ActionResult Post(long id)
		{
			var post = PostService.GetPost(id);

			if (post == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var group = UserGroupService.GetGroup(post.GroupId);

			if (group == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var model = new PostModel
			{
				Post = post,
				Group = group,
				IsBooked = CurrentUserId > 0 && post.PostType != (int)PostType.Venue
					? PostService.IsBookedPost(post.Id, CurrentUserId) : false,
				IsConcerned = CurrentUserId > 0
					? PostService.IsUserConcernPost(post.Id, CurrentUserId) : false
			};

			var venueSchedules = CurrentUserId > 0 && post.PostType == (int)PostType.Venue
				? PostService.GetBookingSchedules(CurrentUserId, id, 7) : null;

			ViewBag.CurrentUserId = CurrentUserId;
			ViewBag.IsFollowing = CurrentUserId > 0
				? UserGroupService.IsUserInGroup(CurrentUserId, group.Id) : false;
			ViewBag.Schedules = venueSchedules != null
				? venueSchedules.ObjectToJson() : "[]";

			return View(model);
		}
	}
}
