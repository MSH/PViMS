using System;
using System.ComponentModel;

namespace VPS.Common
{
	public static class Enumerations
	{
		public static T Parse<T>(string enumValue)
		{
			return (T) Enum.Parse(typeof (T), enumValue);
		}

		public static string GetDescription<T>(T value)
		{
			var fi = value.GetType().GetField(value.ToString());

			var attributes =
				(DescriptionAttribute[]) fi.GetCustomAttributes(
					typeof (DescriptionAttribute),
					false);

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			return value.ToString();
		}
	}
}