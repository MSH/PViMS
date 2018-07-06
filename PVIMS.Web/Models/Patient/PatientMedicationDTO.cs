using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientMedicationDTO
    {
        [Key]
        public Guid PatientMedicationIdentifier { get; set; }

        public int PatientMedicationId { get; set; }

        public int PatientId { get; set; }

        [Display(Name = "Medication")]
        public int MedicationId { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public string Dose { get; set; }

        [Display(Name = "Dose Frequency")]
        public string DoseFrequency { get; set; }

        [Display(Name = "Dose Unit")]
        public string DoseUnit { get; set; }

        public DateTime Created { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public string UpdatedBy { get; protected set; }
        public string CreatedBy { get; protected set; }

        public IEnumerable<CustomAttributeDTO> CustomAttributes { get; set; }
    }
}