using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class LabTestUnit : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}