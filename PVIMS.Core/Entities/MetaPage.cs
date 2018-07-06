namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using PVIMS.Core.ValueTypes;

    [Table("MetaPage")]
    public class MetaPage : EntityBase
    {
        public MetaPage()
        {
            Widgets = new HashSet<MetaWidget>();

            IsSystem = false;
            IsVisible = true;
        }

        [Required]
        public Guid metapage_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string PageName { get; set; }

        [StringLength(250)]
        public string PageDefinition { get; set; }

        public string MetaDefinition { get; set; }

        [StringLength(250)]
        public string Breadcrumb { get; set; }

        public bool IsSystem { get; set; }

        public bool IsVisible { get; set; }

        public virtual ICollection<MetaWidget> Widgets { get; set; }

        public bool IsWidgetUnique(MetaWidgetLocation location)
        {
            if (Widgets.Count == 0)
                { return true; }
            else
                { return Widgets.Any(w => w.WidgetLocation == location); };
        }
    }
}
