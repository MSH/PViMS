using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class Config : AuditedEntityBase
    {
        [Required]
        public virtual ConfigType ConfigType { get; set; }

        [Required]
        [StringLength(100)]
        public string ConfigValue { get; set; }
    }
}