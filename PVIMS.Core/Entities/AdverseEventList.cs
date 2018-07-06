using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class AdverseEventList : Entity<int>
    {
        public string Description { get; set; }
        public string Criteria { get; set; }
        public int PatientCount { get; set; }
    }
}
