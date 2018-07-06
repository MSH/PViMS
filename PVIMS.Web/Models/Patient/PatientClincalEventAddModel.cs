using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientClincalEventAddModel
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
        [Display(Name = "Event Description (As stated by patient or reporter)")]
        [StringLength(500, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string SourceDescription { get; set; }
        [Display(Name = "Onset Date")]
        public DateTime? OnsetDate { get; set; }
        [Display(Name = "Resolution Date")]
        public DateTime? ResolutionDate { get; set; }
        [Display(Name = "Event Duration")]
        public string EventDuration { get; set; }
        public CustomAttributeAddModel[] CustomAttributes { get; set; }
    }
}