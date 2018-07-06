using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientMedicationDeleteModel
    {
        [Key]
        public int PatientMedicationId { get; set; }
        public string PatientFullName { get; set; }
        public string Medication { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string Dose { get; set; }
        [Display(Name = "Dose Frequency")]
        public string DoseFrequency { get; set; }
        [Display(Name = "Dose Unit")]
        public string DoseUnit { get; set; }
        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}