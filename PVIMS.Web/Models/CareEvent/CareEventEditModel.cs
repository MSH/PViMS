using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class CareEventEditModel
    {
        [Key]
        public int CareEventId { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}