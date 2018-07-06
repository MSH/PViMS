using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class Condition : EntityBase
	{
		public Condition()
		{
            ConditionLabTests = new HashSet<ConditionLabTest>();
            ConditionMedications = new HashSet<ConditionMedication>();
            ConditionMedDras = new HashSet<ConditionMedDra>();
			PatientConditions = new HashSet<PatientCondition>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }
        public bool Chronic { get; set; }

        public virtual ICollection<ConditionLabTest> ConditionLabTests { get; set; }
        public virtual ICollection<ConditionMedication> ConditionMedications { get; set; }
        public virtual ICollection<ConditionMedDra> ConditionMedDras { get; set; }
		public virtual ICollection<PatientCondition> PatientConditions { get; set; }

        public bool HasMedDra(List<TerminologyMedDra> meddras)
        {
            if (ConditionMedDras.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (var cm in ConditionMedDras)
                {
                    if (meddras.Contains(cm.TerminologyMedDra)) {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

	}
}