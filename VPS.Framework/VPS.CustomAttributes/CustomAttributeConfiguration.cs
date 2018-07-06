using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using VPS.Common.Domain;

namespace VPS.CustomAttributes
{
	public class CustomAttributeConfiguration : Entity<long>
	{
		public string ExtendableTypeName { get; set; }
		public CustomAttributeType CustomAttributeType { get; set; }
		public string Category { get; set; }
		public string AttributeKey { get; set; }
        [StringLength(150)]
        public string AttributeDetail { get; set; }
        public string AttributeCode { get; set; }
        public string Localisation_1 { get; set; }
        public string Localisation_2 { get; set; }
        public bool IsRequired { get; set; }
        public int? StringMaxLength { get; set; }
        public int? NumericMinValue { get; set; }
        public int? NumericMaxValue { get; set; }
        public bool FutureDateOnly { get; set; }
        public bool PastDateOnly { get; set; }
        public bool IsVisible { get; set; }
	}
}