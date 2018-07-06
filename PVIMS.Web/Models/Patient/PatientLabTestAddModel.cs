using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class PatientLabTestAddModel
    {
        [Key]
        public int PatientId { get; set; }
        public string PatientFullName { get; set; }
        [Required]
        [Display(Name = "Test or Procedure")]
        public int LabTestId { get; set; }
        [Required]
        [Display(Name = "Test Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
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
        public CustomAttributeAddModel[] CustomAttributes { get; set; }

        public DateTime Today { get { return DateTime.Today; } } 
    }
}