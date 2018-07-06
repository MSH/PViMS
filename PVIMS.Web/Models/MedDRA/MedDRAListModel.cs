using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class MedDRAListModel
    {
        [Display(Name = "Term Type")]
        public string TermType { get; set; }
        [Display(Name = "Find by Term")]
        public string FindTerm { get; set; }

        public HttpPostedFileBase InputFile { get; set; }
        public string Summary { get; set; }
        [Display(Name = "Current MedDRA Version")]
        public string CurrentVersion { get; set; }

        public MedDRAListItemModel[] ListItems { get; set; }
    }
}