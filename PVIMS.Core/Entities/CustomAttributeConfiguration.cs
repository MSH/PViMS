using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;
using VPS.CustomAttributes;

namespace PVIMS.Core.Entities
{
    public class CustomAttributeConfiguration : Entity<int>
    {
        public string ExtendableTypeName { get; set; }
        public CustomAttributeType CustomAttributeType { get; set; }
        public string Category { get; set; }
        public string AttributeKey { get; set; }
        [StringLength(150)]
        public string AttributeDetail { get; set; }
        public bool IsRequired { get; set; }
        public int? StringMaxLength { get; set; }
        public int? NumericMinValue { get; set; }
        public int? NumericMaxValue { get; set; }
        public bool FutureDateOnly { get; set; }
        public bool PastDateOnly { get; set; }
        public bool IsSearchable { get; set; }
    }
}
