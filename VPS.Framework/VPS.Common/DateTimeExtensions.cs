using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common
{
	public static class DateTimeExtensions
	{
		public static int AgeAt(this DateTime date, DateTime other)
		{
			return other.Year - date.Year + (date.DayOfYear < other.DayOfYear ? -1 : 0);
		}

		public static DateTime ToStartOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static DateTime ToEndOfMonth(this DateTime date)
		{
			return date.ToStartOfMonth().AddMonths(1).AddDays(-1);
		}
	}
}
