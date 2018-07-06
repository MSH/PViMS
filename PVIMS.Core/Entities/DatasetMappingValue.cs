using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class DatasetMappingValue : EntityBase
    {
        [Required]
        public virtual DatasetMapping Mapping { get; set; }
        public virtual DatasetMappingSub SubMapping { get; set; }

        [Required]
        [StringLength(100)]
        public string SourceValue { get; set; }

        [Required]
        [StringLength(100)]
        public string DestinationValue { get; set; }

        public bool Active { get; set; }
    }
}