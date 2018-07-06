using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class DrugList : Entity<int>
    {
        public int MedicationId { get; set; }
        public string DrugName { get; set; }
        public int PatientCount { get; set; }
    }
}
