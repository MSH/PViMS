namespace PVIMS.Core.Entities
{
    using PVIMS.Core.ValueTypes;

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MetaReport")]
    public class MetaReport : EntityBase
    {
        public MetaReport()
        {
            metareport_guid = Guid.NewGuid();
            Breadcrumb = "** NOT DEFINED **";
            ReportStatus = MetaReportStatus.Unpublished;
            IsSystem = false;
        }

        [Required]
        public Guid metareport_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string ReportName { get; set; }

        [StringLength(250)]
        public string ReportDefinition { get; set; }

        public string MetaDefinition { get; set; }

        [StringLength(250)]
        public string Breadcrumb { get; set; }

        public string SQLDefinition { get; set; }

        public bool IsSystem { get; set; }

        [Required]
        public virtual MetaReportStatus ReportStatus { get; set; }

    }
}
