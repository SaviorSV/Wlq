using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Service;

namespace Wlq.Web.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
			var commonService = Hanger.Common.LocalServiceLocator.GetService<ICommonService>();

            return View();
        }

    }
}
