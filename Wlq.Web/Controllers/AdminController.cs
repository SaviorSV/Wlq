using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Wlq.Domain;
using Wlq.Service;

namespace Wlq.Web.Controllers
{
    public class AdminController : BaseController
    {
		private UserInfo _adminUser;

		protected UserInfo AdminUser
		{
			get
			{
				if (_adminUser == null)
				{
					if (CurrentUser != null && CurrentUser.IsAdmin)
					{
						_adminUser = CurrentUser;
					}
				}

				return _adminUser;
			}
		}

        public ActionResult Index()
        {
            return View();
        }

    }
}
