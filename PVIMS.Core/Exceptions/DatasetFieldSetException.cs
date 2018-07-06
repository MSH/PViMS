using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVIMS.Core.Exceptions
{
    public class DatasetFieldSetException : Exception
    {
        public DatasetFieldSetException(string key, string message)
            :base(message)
        {
            this.Key = key;
        }

        public string Key { get; private set; }
    }
}
