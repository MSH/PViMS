namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaColumnType")]
    public class MetaColumnType : EntityBase
    {
        [Required]
        public Guid metacolumntype_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
