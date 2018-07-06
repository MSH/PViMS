using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class LabResult : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}