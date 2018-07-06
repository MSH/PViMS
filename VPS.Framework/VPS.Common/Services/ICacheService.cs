using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPS.Common.Services
{
    public interface ICacheService
    {
        T Get<T>(string key);
        bool Add<T>(string key, T value);
        bool Exists(string key);
        void Clear();
    }
}
