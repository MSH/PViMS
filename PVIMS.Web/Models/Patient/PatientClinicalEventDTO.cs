using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientClinicalEventDTO
    {
        public Guid PatientClinicalEventIdentifier { get; set; }
        public int PatientClinicalEventId { get; set; }

        public int PatientId { get; set; }

        public int MedDraId { get; set; }

        public string Description { get; set; }
        
        public DateTime? OnsetDate { get; set; }
       
        public DateTime? ResolutionDate { get; set; }

        public DateTime? ReportedDate { get; set; }

        public DateTime Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }

        public IEnumerable<CustomAttributeDTO> customAttributes { get; set; }
    }
}