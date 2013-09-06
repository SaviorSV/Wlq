using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;

using Wlq.Persistence;
using Wlq.Persistence.Migrations;
using Wlq.Web.Fliters;

namespace Wlq.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleExceptionAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			Hanger.Common.HangerFramework.Start();

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
		}

		protected void Application_End()
		{
			Hanger.Common.HangerFramework.End();
		}
	}
}