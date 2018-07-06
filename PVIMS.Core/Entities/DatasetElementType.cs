using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PVIMS.Core.Entities
{
	public class DatasetElementType : EntityBase
	{
		public DatasetElementType()
		{
			DatasetElements = new HashSet<DatasetElement>();
		}

		[Required]
		[StringLength(50)]
		public string Description { get; set; }

		public virtual ICollection<DatasetElement> DatasetElements { get; set; }
	}
}