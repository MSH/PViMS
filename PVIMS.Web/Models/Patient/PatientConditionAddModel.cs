using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class PatientConditionAddModel
    {
        [Key]
        public int PatientId { get; set; }
        [Display(Name = "Find By Term")]
        public string FindTerm { get; set; }
        [Display(Name = "Term Type")]
        public string TermType { get; set; }
        public string TermResult { get; set; }
        [Display(Name = "Term Results")]
        public IEnumerable<TerminologyMedDRA> TermResults { get; set; }
        public string PatientFullName { get; set; }
        [Display(Name = "Condition Outcome")]
        public int? OutcomeId { get; set; }
        [Display(Name = "Condition Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [Display(Name = "Condition Outcome Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Treatment Outcome")]
        public int? TreatmentOutcomeId { get; set; }
        public CustomAttributeAddModel[] CustomAttributes { get; set; }
        [StringLength(500)]
        public string Comments { get; set; }

        public DateTime Today { get { return DateTime.Today; } }
    }
}