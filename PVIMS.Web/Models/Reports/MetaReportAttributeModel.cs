using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public enum ViewType
    {
        List = 1,
        Summary = 2,
        Filter = 3
    }

    public class MetaReportAttributeModel
    {
        public MetaReportAttributeModel()
        {
            ListItems = new HashSet<ListItem>();
            StratifyItems = new HashSet<ListItem>();
            FilterItems = new HashSet<FilterItem>();
        }

        [Key]
        public int MetaReportId { get; set; }

        [Display(Name = "Report Name")]
        public string ReportName { get; set; }
        [Display(Name = "Report Definition")]
        public string ReportDefinition { get; set; }

        public int ReportType { get; set; }
        [Display(Name = "Report Type")]
        public string ReportTypeDisplay { get; set; }
        public ViewType ViewType { get; set; }

        public int CoreEntity { get; set; }
        [Display(Name = "Core Entity")]
        public string CoreEntityDisplay { get; set; }

        [Display(Name = "Meta Column")]
        public int MetaColumnForListId { get; set; }
        [Display(Name = "Display Name")]
        [StringLength(50)]
        public string DisplayForList { get; set; }
        public virtual ICollection<ListItem> ListItems { get; set; }

        [Display(Name = "Meta Column")]
        public int MetaColumnForSummaryId { get; set; }
        [Display(Name = "Display Name")]
        [StringLength(50)]
        public string DisplayForSummary { get; set; }
        public virtual ICollection<ListItem> StratifyItems { get; set; }

        [Display(Name = "Meta Column")]
        public int MetaColumnForFilterId { get; set; }
        public string Relation { get; set; }
        public string Operator { get; set; }
        public virtual ICollection<FilterItem> FilterItems { get; set; }

        public class ListItem
        {
            public int MetaColumnId { get; set; }
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
        }

        public class FilterItem
        {
            public int MetaColumnId { get; set; }
            public string Relation { get; set; }
            public string AttributeName { get; set; }
            public string Operator { get; set; }
        }
    }
}
