using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class Medication : EntityBase
	{
		public Medication()
		{
			ConditionMedications = new HashSet<ConditionMedication>();
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
			PatientMedications = new HashSet<PatientMedication>();
		}

		[Required]
		[StringLength(100)]
		public string DrugName { get; set; }

		public bool Active { get; set; }
		public int PackSize { get; set; }

		[Required]
		[StringLength(40)]
		public string Strength { get; set; }

		[StringLength(10)]
		public string CatalogNo { get; set; }

        public string FullName
        {
            get
            {
                return DrugName;
            }
        }

		public virtual ICollection<ConditionMedication> ConditionMedications { get; set; }
		public virtual MedicationForm MedicationForm { get; set; }
		public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
		public virtual ICollection<PatientMedication> PatientMedications { get; set; }
	}
}