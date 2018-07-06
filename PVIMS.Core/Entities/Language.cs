using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class Language : EntityBase
	{
		public Language()
		{
			PatientLanguages = new HashSet<PatientLanguage>();
		}

		[StringLength(20)]
		public string Description { get; set; }

		public virtual ICollection<PatientLanguage> PatientLanguages { get; set; }
	}
}