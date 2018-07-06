namespace PVIMS.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CohortGroup : EntityBase
    {
        public CohortGroup()
        {
            CohortGroupEnrolments = new HashSet<CohortGroupEnrolment>();
            EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
        }

        [Required]
        [StringLength(50)]
        public string CohortName { get; set; }

        [Required]
        [StringLength(5)]
        public string CohortCode { get; set; }

        public int LastPatientNo { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public int MinEnrolment { get; set; }

        public int MaxEnrolment { get; set; }

        public virtual Condition Condition { get; set; }

        public virtual ICollection<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }

        public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }

        public string DisplayName
        {
            get
            {
                return String.Format("{0} ({1})", CohortName, CohortCode);
            }
        }
    }
}
