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

		public static MvcHtmlString IsChecked(this HtmlHelper helper, bool isChecked)
		{
			if (isChecked)
				return new MvcHtmlString("checked='checked'");

			return null;
		}

		public static MvcHtmlString IsVisible(this HtmlHelper helper, bool isVisible)
		{
			if (!isVisible)
				return new MvcHtmlString("style='display:none;'");

			return null;
		}
	}
}
