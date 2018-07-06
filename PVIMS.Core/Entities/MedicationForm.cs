using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class MedicationForm : EntityBase
	{
		public MedicationForm()
		{
			Medications = new HashSet<Medication>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<Medication> Medications { get; set; }
	}
}