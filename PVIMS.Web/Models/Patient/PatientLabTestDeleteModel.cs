using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class PatientLabTestDeleteModel
    {
        [Key]
        public int PatientLabTestId { get; set; }
        public string PatientFullName { get; set; }
        [Display(Name = "Test or Procedure")]
        public string LabTest { get; set; }
        [Required]
        [Display(Name = "Test Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TestDate { get; set; }
        [Display(Name = "Test Result (Coded)")]
        public string TestResult { get; set; }
        [Display(Name = "Test Result (Value)")]
        [StringLength(20)]
        public string LabValue { get; set; }
        [Display(Name = "Reference Range - Lower Limit")]
        [StringLength(20)]
        public string ReferenceLower { get; set; }
        [Display(Name = "Reference Range - Upper Limit")]
        [StringLength(20)]
        public string ReferenceUpper { get; set; }
        [Display(Name = "Test Unit")]
        public string LabTestUnit { get; set; }
        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}