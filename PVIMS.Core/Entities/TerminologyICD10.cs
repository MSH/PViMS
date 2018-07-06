using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class TerminologyIcd10 : EntityBase
	{
		[StringLength(20)]
		public string Name { get; set; }

		public string Description { get; set; }
	}
}