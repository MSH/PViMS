using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class CareEventAddModel
    {
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}