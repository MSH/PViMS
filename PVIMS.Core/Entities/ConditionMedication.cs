namespace PVIMS.Core.Entities
{
	public class ConditionMedication : EntityBase
	{
		public virtual Condition Condition { get; set; }
		public virtual Medication Medication { get; set; }
	}
}