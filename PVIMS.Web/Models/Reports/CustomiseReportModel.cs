using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class CustomiseReportModel
    {
        public CustomiseReportModel()
        {
            ListAttributeItems = new HashSet<ListAttributeItem>();
            FilterAttributeItems = new HashSet<FilterAttributeItem>();
        }

        [Key]
        public int MetaReportId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }

        [Display(Name = "Report Definition")]
        [StringLength(250)]
        public string ReportDefinition { get; set; }

        public ViewType ViewType { get; set; }

        [Required]
        [Display(Name = "Report Type")]
        public int ReportType { get; set; }
        [Required]
        [Display(Name = "Core Entity")]
        public int CoreEntity { get; set; }
        [Required]
        [Display(Name = "Report Status")]
        public int ReportStatus { get; set; }

        public virtual ICollection<ListAttributeItem> ListAttributeItems { get; set; }
        public virtual ICollection<FilterAttributeItem> FilterAttributeItems { get; set; }

        public bool AllowDeletion { get; set; }

        public class ListAttributeItem
        {
            public string Attribute { get; set; }
            [Display(Name = "Display")]
            public string DisplayName { get; set; }
        }

        public class FilterAttributeItem
        {
            public string Relationship { get; set; }
            public string Attribute { get; set; }
            public string Operator { get; set; }
            [Display(Name = "Field Value")]
            public string FieldValue { get; set; }
        }

    }
}