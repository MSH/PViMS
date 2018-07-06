using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VPS.Common.Utilities
{
    public static class TypeConverter
    {
	    static TypeConverter()
	    {
			AllowedDateFormats = new List<string>();    
	    }

		public static ICollection<string> AllowedDateFormats { get; private set; }


        public static T Convert<T>(string value)
        {
            var type = typeof(T);

            if (type.IsPrimitive)
                return GetPrimitiveType<T>(value);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var typeArgs = type.GetGenericArguments();

                if (typeArgs.Count() != 1) return default(T);

                var underlyingType = typeArgs.First();

                if (underlyingType.IsPrimitive)
                    return GetPrimitiveType<T>(value);
            }

            return default(T);
        }

        private static T GetPrimitiveType<T>(string value)
        {
            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        public static object Convert(Type targetType, string value)
        {
            if (IsNullableType(targetType))
            {
                return Convert(Nullable.GetUnderlyingType(targetType), value);
            }

            switch (targetType.Name)
            {
                case "DateTime":
                    return ConvertToDate(value);
                case "Int32":
                    return System.Convert.ToInt32(value);
                case "Int64":
                    return System.Convert.ToInt64(value);
                case "Int16":
                    return System.Convert.ToInt16(value);
                case "Boolean":
                    return System.Convert.ToBoolean(value);
                case "Single":
                    return System.Convert.ToSingle(value);
                case "Double":
                    return System.Convert.ToDouble(value);
                case "Decimal":
                    return System.Convert.ToDecimal(value);
            }

            return value;
        }

	    private static DateTime ConvertToDate(string value)
	    {
		    return DateTime.ParseExact(value, AllowedDateFormats.ToArray(), CultureInfo.InvariantCulture,
			    DateTimeStyles.AllowWhiteSpaces);
	    }

	    private static bool IsNullableType(Type targetType)
        {
            return targetType.Name == "Nullable`1";
        }
    }
}
