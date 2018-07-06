using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class PatientList : Entity<int>
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string FacilityName { get; set; }
    }
}
