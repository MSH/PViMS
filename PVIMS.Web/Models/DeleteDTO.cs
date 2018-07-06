using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DeleteDTO
    {
        [Required]
        public int Id { get; set; }
        public string Reason { get; set; }
    }
}