using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class MetaReportItem
    {
        [Key]
        public int MetaReportId { get; set; }

        public string GUID { get; set; }
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }
        public string Definition { get; set; }
        public string Status { get; set; }
    }
}
