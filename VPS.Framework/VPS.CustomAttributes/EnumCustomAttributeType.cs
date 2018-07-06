using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.CustomAttributes
{
    public enum CustomAttributeType
    {
        None = 0,
        Numeric = 1,
        String = 2,
        Selection = 3,
        DateTime = 4,
        FirstClassProperty = 5
    }
}
