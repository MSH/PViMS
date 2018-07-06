using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Collections
{
    public interface IPagingInfo
    {
        int FirstResult { get; set; }
        int MaxResults { get; set; }
    }
}
