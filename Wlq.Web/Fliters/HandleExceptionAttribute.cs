using System.Web.Mvc;

using Hanger.Common;

namespace Wlq.Web.Fliters
{
	public class HandleExceptionAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			var controllerName = filterContext.RouteData.Values["controller"];
			var actionName = filterContext.RouteData.Values["action"];

			LocalLoggingService.Exception(string.Format("Controller:{0}, Action:{1} Error!", controllerName, actionName));
			LocalLoggingService.Exception(filterContext.Exception);

#if DEBUG
#else
			filterContext.ExceptionHandled = true;

			filterContext.Result = new ViewResult
			{
				ViewName = "_Error"
			};
#endif
			//base.OnException(filterContext);
		}
	}
}