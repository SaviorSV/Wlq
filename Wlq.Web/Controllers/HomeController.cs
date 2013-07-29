using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wlq.Web.Controllers
{
    public class HomeController : BaseController
    {
       
        public ActionResult Index()
        {
			var str = CommonService.ToString() + "<br />" + CommonService.ToString();

			return Content(str);
        }

    }
}
