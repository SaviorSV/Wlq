using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Hanger.Common;

namespace Wlq.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Activity()
		{
			return View();
		}

		public ActionResult Course()
		{
			return View();
		}

		public ActionResult Venue()
		{
			return View();
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
