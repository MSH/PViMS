using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PVIMS.Core.Entities
{
    [Table("RiskFactor")]
    public class RiskFactor : EntityBase
    {
        public RiskFactor()
        {
            Options = new HashSet<RiskFactorOption>();
        }

        [Required]
        [StringLength(50)]
        public string FactorName { get; set; }
        [Required]
        public string Criteria { get; set; }
        [StringLength(20)]
        public string Display { get; set; }
        
        public bool IsSystem { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<RiskFactorOption> Options { get; set; }
    }
}