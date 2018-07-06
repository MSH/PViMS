using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PVIMS.Web.Models
{
    public class PatientDeleteModel
    {
        [Key]
        public int PatientId { get; set; }
        [Display(Name = "Patient Name")]
        public string PatientFullName { get; set; }
        [Required]
        [Display(Name = "Reason")]
        [StringLength(200, ErrorMessage = "{0} must be at most {1} characters long.")]
        public string ArchiveReason { get; set; }
    }
}