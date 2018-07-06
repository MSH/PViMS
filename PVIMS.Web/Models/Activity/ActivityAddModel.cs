using System;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class ActivityAddModel
    {
        [Key]
        public int ActivityInstanceId { get; set; }

        public string Comments { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Current Status")]
        public string CurrentExecutionStatus { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "New Status")]
        public string NewExecutionStatus { get; set; }

        [Display(Name = "Receipt Date")] // render as date so that it can appear in the edit form
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ContextDate { get; set; }

        [StringLength(20)]
        [Display(Name = "Receipt Code")]
        public string ContextCode { get; set; }
    }
}