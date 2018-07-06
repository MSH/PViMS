using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPS.Common.Domain;

namespace VPS.Common.Utilities
{
    public static class StringExtensions
    {
        public static string ToFullCacheKey(this string name, CacheKey cacheKey)
        {
            return cacheKey + ":" + name;
        }
    }
}
