using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class FieldValue : EntityBase
	{
		[Required]
		[StringLength(100)]
		public string Value { get; set; }

		public bool Default { get; set; }
		public bool Other { get; set; }
		public bool Unknown { get; set; }
		public virtual Field Field { get; set; }
	}
}