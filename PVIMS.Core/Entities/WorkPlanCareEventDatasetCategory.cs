namespace PVIMS.Core.Entities
{
	public class WorkPlanCareEventDatasetCategory : EntityBase
	{
		public virtual DatasetCategory DatasetCategory { get; set; }
		public virtual WorkPlanCareEvent WorkPlanCareEvent { get; set; }
	}
}