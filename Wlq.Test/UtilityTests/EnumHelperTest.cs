using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Wlq.Domain;
using Wlq.Service.Utility;

namespace Wlq.Test.UtilityTests
{
	public class EnumHelperTest
	{
		[Test]
		public void ConvertorTest()
		{
			var value = EnumHelper.GetDescription<PostType>(PostType.Venue);

			Assert.AreEqual("场地", value);
		}
	}
}
