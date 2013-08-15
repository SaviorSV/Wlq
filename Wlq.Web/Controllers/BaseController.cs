using System.Web.Mvc;

using Hanger.Common;
using Microsoft.Practices.Unity;
using Wlq.Persistence;
using Wlq.Service;
using Wlq.Domain;
using System.Web;

namespace Wlq.Web.Controllers
{
    public class BaseController : Controller
	{
		#region Services

		private DatabaseContext _databaseContext;

		private DatabaseContext DatabaseContext
		{
			get
			{
				if (_databaseContext == null)
					_databaseContext = new DatabaseContext();

				return _databaseContext;
			}
		}

		private ICommonService _commonService;

		protected ICommonService CommonService
		{
			get { return this.GetService<ICommonService>(ref _commonService); }
		}

		private IUserService _userService;

		protected IUserService UserService
		{
			get { return this.GetService<IUserService>(ref _userService); }
		}

		private IPostService _postService;

		protected IPostService PostService
		{
			get { return this.GetService<IPostService>(ref _postService); }
		}

		private TService GetService<TService>(ref TService service)
		{
			if (service == null)
			{
				service = LocalServiceLocator.GetService<TService>(
					new ParameterOverrides { { "databaseContext", this.DatabaseContext } }
				);
			}

			return service;
		}

		#endregion

		#region Loggin User

		private long _currentUserId;

		protected long CurrentUserId
		{
			get
			{
				if (HttpContext.User.Identity.IsAuthenticated)
				{
					if (_currentUserId == 0)
					{
						long.TryParse(HttpContext.User.Identity.Name, out _currentUserId);
					}
				}

				return _currentUserId;
			}
		}

		private UserInfo _user = null;

		protected UserInfo CurrentUser
		{
			get
			{
				if (CurrentUserId > 0)
				{
					if (_user == null)
					{
						_user = UserService.GetUser(CurrentUserId);
					}

					return _user;
				}

				return null;
			}
		}

		#endregion

		public BaseController() { }

		public ActionResult AlertAndRedirect(string message, string url)
		{
			return RedirectToAction("Redirect", "Common", new { Message = HttpUtility.UrlEncode(message), Url = url });
		}

		protected override void Dispose(bool disposing)
		{
			if (_databaseContext != null)
				_databaseContext.Dispose();

			base.Dispose(disposing);
		}
    }
}
