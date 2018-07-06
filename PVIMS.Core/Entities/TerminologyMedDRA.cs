using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class TerminologyMedDra : EntityBase
	{
		public TerminologyMedDra()
		{
			PatientClinicalEvents = new HashSet<PatientClinicalEvent>();
			Children = new HashSet<TerminologyMedDra>();
            Scales = new HashSet<MedDRAScale>();
            ConditionMedDras = new HashSet<ConditionMedDra>();

            Common = false;
		}

		[Required]
		[StringLength(100)]
		public string MedDraTerm { get; set; }

		[Required]
		[StringLength(10)]
		public string MedDraCode { get; set; }

		[Required]
		[StringLength(4)]
		public string MedDraTermType { get; set; }

        [StringLength(7)]
        public string MedDraVersion { get; set; }

        [Required]
        public bool Common { get; set; }

        public virtual TerminologyMedDra Parent { get; set; }

		public virtual ICollection<PatientClinicalEvent> PatientClinicalEvents { get; set; }
		[InverseProperty("Parent")]
		public virtual ICollection<TerminologyMedDra> Children { get; set; }
        public virtual ICollection<ConditionMedDra> ConditionMedDras { get; set; }
        public virtual ICollection<MedDRAScale> Scales { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                //return MedDraTerm + '(' + MedDraCode + ')';
                return MedDraTerm;
            }
        }
	}
}