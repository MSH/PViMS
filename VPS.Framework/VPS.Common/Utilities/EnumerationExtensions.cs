using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Utilities
{
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Determines whether the specified enumeration value is in the specified collection to check.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="collectionToCheck">The collection to check.</param>
        /// <returns></returns>
        public static bool IsIn(this Enum value, params Enum[] collectionToCheck)
        {
            return collectionToCheck.Contains(value);
        }
    }
}
