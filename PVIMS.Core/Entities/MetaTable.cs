namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaTable")]
    public class MetaTable : EntityBase
    {
        public MetaTable()
        {
            Columns = new HashSet<MetaColumn>();
        }

        [Required]
        public Guid metatable_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string TableName { get; set; }

        [Required]
        public virtual MetaTableType TableType { get; set; }

        [StringLength(100)]
        public string FriendlyName { get; set; }

        [StringLength(250)]
        public string FriendlyDescription { get; set; }

        public virtual ICollection<MetaColumn> Columns { get; set; }
    }
}
