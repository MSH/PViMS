namespace PVIMS.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using PVIMS.Core.ValueTypes;
    
    [Table("MetaWidget")]
    public class MetaWidget : EntityBase
    {
        public MetaWidget()
        {
            metawidget_guid = Guid.NewGuid();

            WidgetStatus = MetaWidgetStatus.Unpublished;
            WidgetLocation = MetaWidgetLocation.Unassigned;
        }

        [Required]
        public Guid metawidget_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string WidgetName { get; set; }

        [Required]
        public virtual MetaWidgetType WidgetType { get; set; }

        [Required]
        public virtual MetaWidgetStatus WidgetStatus { get; set; }

        [Required]
        public virtual MetaWidgetLocation WidgetLocation { get; set; }

        [Required]
        public virtual MetaPage MetaPage { get; set; }

        [StringLength(250)]
        public string WidgetDefinition { get; set; }

        public string Content { get; set; }

        public string Icon { get; set; }

    }
}
