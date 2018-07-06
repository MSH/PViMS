using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using VPS.CustomAttributes;

namespace PVIMS.Core.Models
{
    public class CustomAttributeConfigDetail
    {
        public long CustomAttributeConfigId { get; set; }
        [DisplayName("Entity Name")]
        public string EntityName { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        [DisplayName("Attribute Name")]
        public string AttributeName { get; set; }
        [StringLength(150)]
        [DisplayName("Attribute Detail")]
        public string AttributeDetail { get; set; }
        [DisplayName("Attribute Type")]
        public CustomAttributeType CustomAttributeType { get; set; }
        public bool Required { get; set; }
        [DisplayName("Maximum Length")]
        public int? StringMaxLength { get; set; }
        [DisplayName("Minimum Value")]
        public int? NumericMinValue { get; set; }
        [DisplayName("Maximum Value")]
        public int? NumericMaxValue { get; set; }
        [DisplayName("Allow Future Dates only")]
        public bool FutureDateOnly { get; set; }
        [DisplayName("Allow Past Dates only")]
        public bool PastDateOnly { get; set; }
        public bool Searchable { get; set; }
    }
}
