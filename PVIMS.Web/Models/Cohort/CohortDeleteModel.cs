using System.ComponentModel.DataAnnotations;

namespace PVIMS.Web.Models
{
    public class CohortDeleteModel
    {
        [Key]
        public int CohortId { get; set; }
        public string CohortName { get; set; }
    }
}