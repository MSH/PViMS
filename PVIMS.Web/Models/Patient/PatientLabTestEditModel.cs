using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class PatientLabTestEditModel
    {
        [Key]
        public int PatientLabTestId { get; set; }
        public string PatientFullName { get; set; }
        [Display(Name = "Test or Procedure")]
        public string LabTest { get; set; }
        [Required]
        [Display(Name = "Test Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [LessThanOrEqualTo("Today", ErrorMessage = "Test Date should be before current date")]
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
        public int? LabTestUnitId { get; set; }
        public CustomAttributeEditModel[] CustomAttributes { get; set; }

        public DateTime Today { get { return DateTime.Today; } } 
    }
}