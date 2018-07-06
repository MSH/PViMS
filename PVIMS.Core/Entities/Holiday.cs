using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class Holiday : EntityBase
    {
        [Required]
        public DateTime HolidayDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
