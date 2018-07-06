using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Services
{
    public interface ICacheManger
    {
        void Load();
        void Clear();
    }
}
