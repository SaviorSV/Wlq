﻿using System;
using System.Web;
using System.Web.Mvc;

using Hanger.Common;
using Wlq.Domain;
using Wlq.Web.Models;

namespace Wlq.Web.Controllers
{
	public class CommonController : BaseController
	{
		public ActionResult Redirect(string message, string url)
		{
			ViewBag.Message = HttpUtility.UrlDecode(message);
			ViewBag.Url = url;

			return PartialView("_Redirect");
		}

		[HttpPost]
		public ActionResult Upload(string type)
		{
			var result = CommonService.UploadTempFile(CurrentUserId, type);

			return Content(result.ObjectToJson());
		}

		public ActionResult Header()
		{
			var isLogin = CurrentUser != null;

			ViewBag.IsLogin = isLogin;

			if (isLogin)
				ViewBag.Name = CurrentUser.Name;
			else
				ViewBag.Name = string.Empty;

			return PartialView("_Header");
		}

		[OutputCache(Duration = 3600)]
		public ActionResult LeftMenu()
		{
			var model = new LeftMenuModel();
			var totalNumber = 0;

			model.Departments = UserGroupService.GetGroupsByParent(0);
			model.Circles = UserGroupService.GetCircles(1, 9, out totalNumber);

			return PartialView("_LeftMenu", model);
		}

		#region ajax

		[HttpPost]
		public ActionResult GetBookingSchedules(long postId, long venueId)
		{
			if (CurrentUserId == 0)
			{
				return Content("[]", "text/json");
			}

			var venueSchedules = PostService.GetBookingSchedules(CurrentUserId, postId, venueId, 7);

			return Content(venueSchedules != null ? venueSchedules.ObjectToJson() : "[]", "text/json");
		}

		[HttpPost]
		public ActionResult Booking(long postId, string bookingDate, int bookingType = 1, int involvedType = 1, long venueId = 0, long venueConfigId = 0)
		{
			var success = false;
			var message = string.Empty;
			var date = DateTime.Now;

			if (CurrentUser != null && DateTime.TryParse(bookingDate, out date))
			{
				var booking = new BookingInfo
				{
					UserId = CurrentUser.Id,
					PostId = postId,
					Name = CurrentUser.Name,
					Mobile = CurrentUser.Mobile,
					VenueId = venueId,
					VenueConfigId = venueConfigId,
					BookingDate = date,
					BookingType = bookingType,
					InvolvedType = involvedType
				};

				success = PostService.Booking(booking, out message);
			}

			return Content(new { Success = success, Message = message }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult CancelBooking(long postId, string bookingDate, long venueId = 0, long venueConfigId = 0)
		{
			var success = false;
			var date = DateTime.Now;

			if (CurrentUserId > 0 && DateTime.TryParse(bookingDate, out date))
			{
				success = PostService.CancelBooking(CurrentUserId, postId, venueConfigId, date);
			}

			return Content(new { Success = success, Message = string.Empty }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult JoinGroup(long groupId)
		{
			var success = false;

			if (CurrentUserId > 0)
			{
				success = UserGroupService.AddUserToGroup(CurrentUserId, groupId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult QuitGroup(long groupId)
		{
			var success = false;

			if (CurrentUserId > 0)
			{
				success = UserGroupService.RemoveUserFromGroup(CurrentUserId, groupId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult ConcernPost(long postId)
		{
			var success = false;

			if (CurrentUserId > 0)
			{
				success = PostService.ConcernPost(postId, CurrentUserId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult UnconcernPost(long postId)
		{
			var success = false;

			if (CurrentUserId > 0)
			{
				success = PostService.UnConcernPost(postId, CurrentUserId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		[HttpPost]
		public ActionResult DeleteMessage(long messageId)
		{
			var success = false;

			if (CurrentUserId > 0)
			{
				success = PostService.DeleteMessage(CurrentUserId, messageId);
			}

			return Content(new { Success = success }.ObjectToJson(), "text/json");
		}

		#endregion
	}
}
