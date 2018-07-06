using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class MedDRAListItemModel
    {
        [Key]
        public int MedDRAId { get; set; }
        [Display(Name = "Parent Term")]
        public string ParentTerm { get; set; }
        public string Term { get; set; }
        public string Code { get; set; }
        [Display(Name = "Term Type")]
        public string TermType { get; set; }
        public string Version { get; set; }
    }
}