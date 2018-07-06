using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
	public class DatasetCategoryElement : EntityBase
	{
        public DatasetCategoryElement()
        {
            Conditions = new HashSet<DatasetCategoryElementCondition>();
            SourceMappings = new HashSet<DatasetMapping>();
            DestinationMappings = new HashSet<DatasetMapping>();
        }

		public short FieldOrder { get; set; }
		public virtual DatasetCategory DatasetCategory { get; set; }
		public virtual DatasetElement DatasetElement { get; set; }

        [StringLength(150)]
        public string FriendlyName { get; set; }
        [StringLength(350)]
        public string Help { get; set; }

        [StringLength(10)]
        public string UID { get; set; }

        public bool System { get; set; }
        public bool Public { get; set; }
        public bool Acute { get; set; }
        public bool Chronic { get; set; }

        public virtual ICollection<DatasetCategoryElementCondition> Conditions { get; set; }

        [InverseProperty("SourceElement")]
        public virtual ICollection<DatasetMapping> SourceMappings { get; set; }
        [InverseProperty("DestinationElement")]
        public virtual ICollection<DatasetMapping> DestinationMappings { get; set; }
	}
}