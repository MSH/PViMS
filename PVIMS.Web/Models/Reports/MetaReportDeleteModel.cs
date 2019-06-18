using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class MetaReportDeleteModel
    {
        [Key]
        public int MetaReportId { get; set; }

        [Display(Name = "Report Name")]
        public string ReportName { get; set; }
        [Display(Name = "Report Definition")]
        public string ReportDefinition { get; set; }
    }
}