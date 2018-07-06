using System;

namespace PVIMS.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string value)
        {
            try
            {
                decimal decimalValue;
                return decimal.TryParse(value, out decimalValue);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
