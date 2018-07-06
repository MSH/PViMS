using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class Priority : EntityBase
	{
		public Priority()
		{
			Encounters = new HashSet<Encounter>();
		}
		
		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<Encounter> Encounters { get; set; }
	}
}