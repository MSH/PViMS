using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class FacilityListItem
    {
        public long FacilityId { get; set; }
        [Display(Name = "Name")]
        public string FacilityName { get; set; }
        [Display(Name = "Code")]
        public string FacilityCode { get; set; }
        [Display(Name = "Type")]
        public string FacilityType { get; set; }
    }
}
