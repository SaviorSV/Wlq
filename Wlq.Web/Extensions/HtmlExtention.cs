using System.Web.Mvc;

namespace Wlq.Web.Extensions
{
	public static class HtmlExtention
	{
		public static MvcHtmlString IsSelected(this HtmlHelper helper, bool isSelected)
		{
			if (isSelected)
				return new MvcHtmlString("selected='selected'");

			return null;
		}
	}
}
