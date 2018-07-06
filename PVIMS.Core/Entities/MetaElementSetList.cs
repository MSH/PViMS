using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class MetaElementSetList : Entity<int>
    {
        public string Element { get; set; }
        public Int64 Value { get; set; }
    }
}
