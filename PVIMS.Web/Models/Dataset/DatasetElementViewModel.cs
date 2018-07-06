using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetElementViewModel
    {
        public DatasetElementViewModel()
        {
            DatasetElementSubs = new DatasetElementSubViewModel[0];
        }

        [Key]
        public int DatasetElementId { get; set; }
        [Display(Name = "Name")]
        public string DatasetElementName { get; set; }
        [Display(Name = "Name")]
        public string DatasetElementDisplayName { get; set; }
        [Display(Name = "Help")]
        public string DatasetElementHelp { get; set; }
        [Display(Name = "Type")]
        public string DatasetElementType { get; set; }
        [Display(Name = "Value")]
        public string DatasetElementValue { get; set; }
        public bool DatasetElementDisplayed { get; set; }
        public bool DatasetElementChronic { get; set; }
        public bool DatasetElementSystem { get; set; }
        public DatasetElementSubViewModel[] DatasetElementSubs { get; set; }
    }
}