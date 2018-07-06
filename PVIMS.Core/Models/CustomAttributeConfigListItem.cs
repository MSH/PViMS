using System.ComponentModel;

namespace PVIMS.Core.Models
{
    public class CustomAttributeConfigListItem
    {
        public long CustomAttributeConfigId { get; set; }
        [DisplayName("Entity Name")]
        public string EntityName { get; set; }
        public string Category { get; set; }
        [DisplayName("Attribute Name")]
        public string AttributeName { get; set; }
        [DisplayName("Attribute Type")]
        public string AttributeTypeName { get; set; }
        public bool Required { get; set; }
        [DisplayName("Max Length (Text)")]
        public int? StringMaxLength { get; set; }
        [DisplayName("Min Value (Numeric)")]
        public int? NumericMinValue { get; set; }
        [DisplayName("Max Value (Numeric)")]
        public int? NumericMaxValue { get; set; }
        [DisplayName("Future Date Only")]
        public bool? FutureDateOnly { get; set; }
        [DisplayName("Past Date Only")]
        public bool? PastDateOnly { get; set; }
        public bool Searchable { get; set; }
    }
}