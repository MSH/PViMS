using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class FacilityAddModel
    {
        [Required]
        [Display(Name = "Facility Code")]
        [StringLength(10)]
        public string FacilityCode { get; set; }
        [Required]
        [Display(Name = "Facility Name")]
        [StringLength(100)]
        public string FacilityName { get; set; }
        [Display(Name = "Facility Type")]
        public int FacilityTypeId { get; set; }
        [Display(Name = "Region")]
        public int? OrgUnitId { get; set; }
        [StringLength(30)]
        [Display(Name = "Telephone Number")]
        public string TelNumber { get; set; }
        [StringLength(30)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [StringLength(30)]
        [Display(Name = "Fax Number")]
        public string FaxNumber { get; set; }
    }
}