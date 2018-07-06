using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientDTO
    {
        [Key]
        public int PatientId { get; set; }
        public Guid PatientUniqueIdentifier { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientMiddleName { get; set; }
        public string PatientLastName { get; set; }
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}")]
        public DateTime PatientDateOfBirth { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PatientCreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PatientUpdatedDate { get; set; }
        public int FacilityId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public IEnumerable<CustomAttributeDTO> customAttributes { get; set; }
        public string Notes { get; set; }
    }
}