using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class FieldType : EntityBase
	{
		public FieldType()
		{
			Fields = new HashSet<Field>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<Field> Fields { get; set; }
	}
}