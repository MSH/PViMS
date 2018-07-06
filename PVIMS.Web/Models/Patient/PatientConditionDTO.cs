using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientConditionDTO
    {
        public Guid PatientConditionIdentifier { get; set; }
        public int PatientConditionId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Comments { get; set; }

        public int ConditionId { get; set; }
        public int PatientId { get; set; }
        public int MedDraId { get; set; }

        public DateTime Created { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public string UpdatedBy { get; protected set; }
        public string CreatedBy { get; protected set; }

        public IEnumerable<CustomAttributeDTO> customAttributes { get; set; }
    }
}