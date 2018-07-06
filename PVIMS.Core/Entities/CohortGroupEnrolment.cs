using System.ComponentModel;

namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CohortGroupEnrolment : EntityBase
    {
	    public virtual Patient Patient { get; set; }
        public DateTime EnroledDate { get; set; }
        public DateTime? DeenroledDate { get; set; }
        [DefaultValue(false)]
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        [StringLength(200)]
        public string ArchivedReason { get; set; }

        public virtual User AuditUser { get; set; }
        public virtual CohortGroup CohortGroup { get; set; }
    }
}
