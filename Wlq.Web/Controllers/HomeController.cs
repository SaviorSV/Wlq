using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Wlq.Web.Models;
using Hanger.Common;

namespace Wlq.Web.Controllers
{
    public class HomeController : BaseController
    {
		private const int _ListSize = 15;

		public ActionResult Index(int pageIndex = 1, PostType postType = PostType.All)
        {
			var totalNumber = 0;
			var posts = postType == PostType.All
				? PostService.GetLastPosts(pageIndex, _ListSize, out totalNumber)
				: PostService.GetPostsByType(postType, true, pageIndex, _ListSize, out totalNumber);

			ViewBag.PageIndex = pageIndex;
			ViewBag.TotalPage = totalNumber > 0 ? Math.Ceiling((decimal)totalNumber / _ListSize) : 1;
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

		public ActionResult Health()
		{
			return View();
		}

		public ActionResult Group()
		{
			return View();
		}

		public ActionResult GroupList()
		{
			return View();
		}
    }
}
