using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Collections
{
    public class PagingInfo : IPagingInfo
    {
        public int FirstResult { get; set; }
        public int MaxResults { get; set; }
    }
}
