using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class MedDRAGrading : EntityBase
	{
        [Required]
        public virtual MedDRAScale Scale { get; set; }

        [Required]
		[StringLength(20)]
		public string Grade { get; set; }

		[Required]
		[StringLength(100)]
		public string Description { get; set; }
	}
}