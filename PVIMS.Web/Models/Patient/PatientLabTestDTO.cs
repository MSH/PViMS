using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientLabTestDTO
    {
        public Guid PatientLabTestIdentifier { get; set; }
        public int PatientId { get; set; }
        public int PatientLabTestId { get; set; }
        public string TestName { get; set; }
        public DateTime TestDate { get; set; }
        public string TestResult { get; set; }
        public int? TestUnit { get; set; }
        public string LabValue { get; set; }

        public DateTime Created { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public string UpdatedBy { get; protected set; }
        public string CreatedBy { get; protected set; }

        public IEnumerable<CustomAttributeDTO> customAttributes { get; set; }
    }
}