using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class WorkPlan : EntityBase
	{
		public WorkPlan()
		{
			EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
			WorkPlanCareEvents = new HashSet<WorkPlanCareEvent>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

        public virtual Dataset Dataset { get; set; }

		public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }
		public virtual ICollection<WorkPlanCareEvent> WorkPlanCareEvents { get; set; }
	}
}