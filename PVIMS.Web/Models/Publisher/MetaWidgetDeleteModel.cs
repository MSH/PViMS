using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class MetaWidgetDeleteModel
    {
        [Key]
        public int MetaWidgetId { get; set; }
        public string PageName { get; set; }
        public string WidgetName { get; set; }
        public string WidgetType { get; set; }
    }
}