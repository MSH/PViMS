namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MetaReport")]
    public class MetaReport : EntityBase
    {
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
    }
}
