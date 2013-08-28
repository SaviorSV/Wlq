using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

using Wlq.Domain;

namespace Wlq.Web.Fliters
{
	public class LoginAuthenticationAttribute : AuthorizeAttribute
	{
		private RoleLevel _role;
		private string _loginController;
		private string _loginAction;

		public LoginAuthenticationAttribute(RoleLevel role, string loginController, string loginAction)
		{
			_role = role;
			_loginController = loginController;
			_loginAction = loginAction;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			var isLogin = false;
			var roleLevel = 0;

			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				long currentUserId = 0;
				long.TryParse(HttpContext.Current.User.Identity.Name, out currentUserId);

				if (currentUserId > 0)
				{
					isLogin = true;

					var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

					if (authCookie != null)
					{
						var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
						var userData = authTicket.UserData;

						int.TryParse(userData, out roleLevel);
					}
				}
			}

			if (!isLogin || roleLevel < (int)_role)
			{
				filterContext.Result = new RedirectToRouteResult(
					new RouteValueDictionary { { "Controller", _loginController }, { "Action", _loginAction } });
			}

			//base.OnAuthorization(filterContext);
		}
	}
}
