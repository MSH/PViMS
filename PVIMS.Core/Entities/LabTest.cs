using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class LabTest : EntityBase
	{
		public LabTest()
		{
			PatientLabTests = new HashSet<PatientLabTest>();
		}

		public bool Active { get; set; }

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<PatientLabTest> PatientLabTests { get; set; }
	}
}