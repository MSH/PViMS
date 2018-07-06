using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientConditionDeleteModel
    {
        [Key]
        public int PatientConditionId { get; set; }
        public string PatientFullName { get; set; }
        [Display(Name = "MedDRA Terminology")]
        public string TerminologyMedDRA { get; set; }
        [Display(Name = "Condition Outcome")]
        public string ConditionOutcome { get; set; }
        [Display(Name = "Condition Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Display(Name = "Condition Outcome Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Treatment Outcome")]
        public string TreatmentOutcome { get; set; }
        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}