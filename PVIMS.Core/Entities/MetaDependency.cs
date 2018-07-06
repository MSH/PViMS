namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaDependency")]
    public class MetaDependency : EntityBase
    {
        [Required]
        public Guid metadependency_guid { get; set; }

        [Required]
        public virtual MetaTable ParentTable { get; set; }

        [Required]
        [StringLength(50)]
        public string ParentColumnName { get; set; }

        [Required]
        public virtual MetaTable ReferenceTable { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferenceColumnName { get; set; }
    }
}
