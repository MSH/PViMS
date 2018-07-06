using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.CustomAttributes
{
    public class CustomAttributeException : Exception
    {
        public CustomAttributeException(string format, params object[] args)
            :base(string.Format(format, args))
        { 
        }
    }
}
