using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class PatientMedicationAddModel
    {
        [Key]
        public int PatientId { get; set; }
        public string PatientFullName { get; set; }
        [Required]
        [Display(Name = "Medication")]
        public int MedicationId { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Start Date")]
        [LessThanOrEqualTo("Today", ErrorMessage = "Start Date should be before current date")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        public string Dose { get; set; }
        [Display(Name = "Dose Frequency")]
        public string DoseFrequency { get; set; }
        [Display(Name = "Dose Unit")]
        public string DoseUnit { get; set; }
        public CustomAttributeAddModel[] CustomAttributes { get; set; }

        public DateTime Today { get { return DateTime.Today; } }
    }
}