namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    
    public partial class CareEvent : EntityBase
    {
        public CareEvent()
        {
            WorkPlanCareEvents = new HashSet<WorkPlanCareEvent>();
        }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
    }
}
