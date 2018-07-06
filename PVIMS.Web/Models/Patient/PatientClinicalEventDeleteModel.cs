using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PVIMS.Web.Models
{
    public class PatientClinicalEventDeleteModel
    {
        [Key]
        public int PatientClinicalEventId { get; set; }
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
        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}