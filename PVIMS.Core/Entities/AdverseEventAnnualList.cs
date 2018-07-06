using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class AdverseEventAnnualList : Entity<int>
    {
        public int? PeriodYear { get; set; }
        public string FacilityName { get; set; }
        public string MedDraTerm { get; set; }
        public int? PatientCount { get; set; }
    }
}
