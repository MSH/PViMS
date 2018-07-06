using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Foolproof;

namespace PVIMS.Web.Models
{
    public class CohortEditModel
    {
        [Key]
        public int CohortId { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Cohort Name")]
        public string CohortName { get; set; }
        [Required]
        [StringLength(5)]
        [Display(Name = "Cohort Code")]
        public string CohortCode { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Start Date")]
        [LessThanOrEqualTo("Today", ErrorMessage = "Start Date should be before current date")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "End Date")]
        public DateTime? FinishDate { get; set; }
        [Required]
        [Display(Name = "Primary Condition Group")]
        public int ConditionId { get; set; }

        public DateTime Today { get { return DateTime.Today; } }
    }
}