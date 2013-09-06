using System.Web.Mvc;

using Hanger.Common;

namespace Wlq.Web.Fliters
{
	public class HandleExceptionAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
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