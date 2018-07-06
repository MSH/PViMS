using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class TreatmentOutcome : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}