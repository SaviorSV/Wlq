
using System.ComponentModel;

namespace Wlq.Service.Utility
{
	public class EnumHelper
	{
		public static string GetDescription<TEnum>(TEnum value)
			where TEnum : struct
		{
			var description = string.Empty;
			var field = value.GetType().GetField(value.ToString());

			if (field != null)
			{
				var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attributes.Length > 0)
				{
					description = ((DescriptionAttribute)attributes[0]).Description;
				}
			}

			return description;
		}
	}
}
