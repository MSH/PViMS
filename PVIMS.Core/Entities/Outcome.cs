using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class Outcome : EntityBase
	{
		[Required]
		[StringLength(50)]
		public string Description { get; set; }
	}
}