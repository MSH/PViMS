namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    using VPS.Common.Domain;

    [Table("AuditLog")]
    public partial class AuditLog : EntityBase
    {
        [Required]
        public virtual AuditType AuditType { get; set; }
        [Required]
        public DateTime ActionDate { get; set; }

        public virtual User User { get; set; }
        public string Details { get; set; }
        public string Log { get; set; }
    }
}
