namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    using VPS.Common.Domain;

    [Table("DatasetRule")]
    public partial class DatasetRule : EntityBase
    {
        public virtual Dataset Dataset { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }

        [Required]
        public virtual DatasetRuleType RuleType { get; set; }

        public bool RuleActive { get; set; }
    }
}
