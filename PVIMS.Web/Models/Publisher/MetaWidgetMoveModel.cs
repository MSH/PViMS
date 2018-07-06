using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class MetaWidgetMoveModel
    {
        [Key]
        public int MetaWidgetId { get; set; }
        public string WidgetName { get; set; }
        public string CurrentPageName { get; set; }
        [Display(Name = "Destination Page")]
        public int DestinationPageId { get; set; }
    }
}