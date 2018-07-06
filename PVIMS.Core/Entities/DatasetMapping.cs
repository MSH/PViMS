using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public enum MappingType
    {
        ElementToElement,
        ValueToValue,
        AttributeToElement,
        AttributeToValue,
        FirstClassToElement,
        FirstClassToValue
    }

    public class DatasetMapping : EntityBase
    {
        public DatasetMapping()
        {
            Values = new HashSet<DatasetMappingValue>();
            SubMappings = new HashSet<DatasetMappingSub>();
        }

        [Required]
        public virtual DatasetCategoryElement DestinationElement { get; set; }
        public virtual DatasetCategoryElement SourceElement { get; set; }

        public string PropertyPath { get; set; }
        public string Property { get; set; }

        public string Tag { get; set; }

        public MappingType MappingType { get; set; }
        public string MappingOption { get; set; }
            
        public virtual ICollection<DatasetMappingValue> Values { get; set; }
        public virtual ICollection<DatasetMappingSub> SubMappings { get; set; }

    }
}