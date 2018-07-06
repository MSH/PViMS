using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class EncounterTypeAddModel
    {
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(250)]
        public string Help { get; set; }
        [Display(Name = "Work Plan")]
        public int WorkPlanId { get; set; }
    }
}