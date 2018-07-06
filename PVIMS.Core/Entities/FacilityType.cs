using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class FacilityType : EntityBase
	{
		public FacilityType()
		{
			Facilities = new HashSet<Facility>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<Facility> Facilities { get; set; }
	}
}