using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class MetaPageDeleteModel
    {
        [Key]
        public int MetaPageId { get; set; }
        public string PageName { get; set; }
    }
}