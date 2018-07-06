using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class PatientOnStudyList : Entity<int>
    {
        public string FacilityName { get; set; }
        public int FacilityId { get; set; }
        public int PatientCount { get; set; }
        public int PatientWithEventCount { get; set; }
    }
}
