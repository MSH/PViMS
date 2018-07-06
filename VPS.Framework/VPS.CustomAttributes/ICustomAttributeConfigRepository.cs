using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPS.CustomAttributes
{
    public interface ICustomAttributeConfigRepository
    {
        IList<CustomAttributeConfiguration> RetrieveAttributeConfigurationsForType(string typeName);
        IList<string> RetrieveAttributeKeysForType(string typeName);
    }
}
