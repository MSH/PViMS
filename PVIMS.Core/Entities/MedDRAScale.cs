using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class MedDRAScale : EntityBase
	{
        [Required]
        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        [Required]
        public virtual SelectionDataItem GradingScale { get; set; }

        public virtual ICollection<MedDRAGrading> Grades { get; set; }
	}
}