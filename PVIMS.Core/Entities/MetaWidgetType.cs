namespace PVIMS.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MetaWidgetType")]
    public class MetaWidgetType : EntityBase
    {
        [Required]
        public Guid metawidgettype_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
