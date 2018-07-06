namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RiskFactorOption")]
    public class RiskFactorOption : EntityBase
    {
        [Required]
        [StringLength(50)]
        public string OptionName { get; set; }
        [Required]
        [StringLength(250)]
        public string Criteria { get; set; }
        [StringLength(30)]
        public string Display { get; set; }
    }
}
