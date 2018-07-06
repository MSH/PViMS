using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PVIMS.Web.Models
{
    public class DatasetElementDeleteModel
    {
        [Key]
        public int DatasetElementId { get; set; }
        [Display(Name = "Element Name")]
        public string DatasetElementName { get; set; }
        public bool AllowDelete { get; set; }
    }
}