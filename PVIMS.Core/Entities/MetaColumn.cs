namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaColumn")]
    public class MetaColumn : EntityBase
    {
        [Required]
        public Guid metacolumn_guid { get; set; }

        [Required]
        [StringLength(100)]
        public string ColumnName { get; set; }

        [Required]
        public virtual MetaTable Table { get; set; }

        [Required]
        public virtual MetaColumnType ColumnType { get; set; }

        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }

        public string Range { get; set; }
    }
}
