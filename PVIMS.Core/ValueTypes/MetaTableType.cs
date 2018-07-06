namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaTableType")]
    public class MetaTableType : EntityBase
    {
        [Required]
        public Guid metatabletype_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
