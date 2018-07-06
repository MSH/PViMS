using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Web.Models
{
    public class PatientClinicalEventEditModel
    {
        [Key]
        public int PatientClinicalEventId { get; set; }
        [Display(Name = "Find By Term")]
        public string FindTerm { get; set; }
        [Display(Name = "Term Type")]
        public string TermType { get; set; }
        public string TermResult { get; set; }
        [Display(Name = "Term Results")]
        public IEnumerable<TerminologyMedDRA> TermResults { get; set; }
        public string PatientFullName { get; set; }
        [Display(Name = "Source MedDRA Terminology")]
        public string SourceTerminologyMedDRA { get; set; }
        [Required]
        [Display(Name = "Event Description (As stated by patient or reporter)")]
        [StringLength(500, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string SourceDescription { get; set; }
        [Required]
        [Display(Name = "Onset Date")] // render as date so that it can appear in the edit form
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnsetDate { get; set; }
        [Display(Name = "Resolution Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ResolutionDate { get; set; }
        [Display(Name = "Event Duration")]
        public string EventDuration { get; set; }
        [Display(Name = "Age Group")]
        public string EventAgeGroup { get; set; }
        public string MeddraSearch { get; set; }

        public ReportInstance ReportInstance { get; set; }

        public CustomAttributeEditModel[] CustomAttributes { get; set; }
        public List<ActivityExecutionStatusForPatient> ReportingItems { get; set; }
    }
}