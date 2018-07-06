using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class ConfigEditModel
    {
        [Key]
        public int ConfigId { get; set; }
        [Required]
        [StringLength(50)]
        public string ConfigType { get; set; }
        [StringLength(100)]
        [Display(Name = "E2B Version")]
        public string ConfigValue1 { get; set; }
        [StringLength(100)]
        [Display(Name = "WebService Subscriber List")]
        public string ConfigValue2 { get; set; }
        [StringLength(100)]
        [Display(Name = "Assessment Scale")]
        public string ConfigValue3 { get; set; }
        [StringLength(5)]
        [Display(Name = "Report Instance New Alert Count")]
        public string ConfigValue4 { get; set; }
        [StringLength(5)]
        [Display(Name = "Medication Onset Check Period In Weeks")]
        public string ConfigValue5 { get; set; }
    }
}