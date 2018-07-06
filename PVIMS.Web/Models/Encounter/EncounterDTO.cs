using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class EncounterDTO
    {
        [Key]
        public Guid EncounterIdentifier { get; set; }
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EncounterDate { get; set; }
        public string Notes { get; set; }

        public int EncounterPriority { get; set; }
        public int EncounterType { get; set; }

        public DateTime EncounterCreatedDate { get; set; }
        public DateTime? EncounterUpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}