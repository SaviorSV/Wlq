using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Wlq.Domain;
using Hanger.Common;

namespace Wlq.Test.ServiceTests
{
	public class PostServiceTest : TestBase
	{
		[Test]
		public void AddPeriods()
		{
			var config1 = new VenueConfigInfo();
			
			config1.DaysOfWeek = 2;
			config1.BegenTime = 1200;
			config1.EndTime = 1400;
			config1.LimitNumber = 2;
			config1.LastModified = DateTime.Now;

			DatabaseContext.VenueConfigs.Add(config1);

			var result = DatabaseContext.SaveChanges() > 0;

			Assert.IsTrue(result);
		}
	}
}
