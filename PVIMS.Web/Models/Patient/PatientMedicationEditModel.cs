using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class PatientMedicationEditModel
    {
        [Key]
        public int PatientMedicationId { get; set; }
        public string PatientFullName { get; set; }
        public string Medication { get; set; }
        [Display(Name = "Medication Form")]
        public string MedicationForm { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [LessThanOrEqualTo("Today", ErrorMessage = "Start Date should be before current date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string Dose { get; set; }
        [Display(Name = "Dose Frequency")]
        public string DoseFrequency { get; set; }
        [Display(Name = "Dose Unit")]
        public string DoseUnit { get; set; }
        public CustomAttributeEditModel[] CustomAttributes { get; set; }

        public DateTime Today { get { return DateTime.Today; } }
    }
}