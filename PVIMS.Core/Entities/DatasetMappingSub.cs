using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class DatasetMappingSub : EntityBase
    {
        public DatasetMappingSub()
        {
            Values = new HashSet<DatasetMappingValue>();
        }

        [Required]
        public virtual DatasetMapping Mapping { get; set; }

        public virtual DatasetElementSub DestinationElement { get; set; }
        public virtual DatasetElementSub SourceElement { get; set; }

        public string PropertyPath { get; set; }
        public string Property { get; set; }

        public MappingType MappingType { get; set; }
        public string MappingOption { get; set; }

        public virtual ICollection<DatasetMappingValue> Values { get; set; }
    }
}