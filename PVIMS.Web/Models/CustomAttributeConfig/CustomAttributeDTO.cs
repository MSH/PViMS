using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class CustomAttributeDTO
    {
        public long CustomAttributeConfigId { get; set; }

        public string EntityName { get; set; }

        public string Category { get; set; }

        public string AttributeName { get; set; }

        public string AttributeTypeName { get; set; }

        public string currentValue { get; set; }

        public DateTime? lastUpdated { get; set; }

        public string lastUpdatedUser { get; set; }

        public bool Required { get; set; }
        
        public int? StringMaxLength { get; set; }
       
        public int? NumericMinValue { get; set; }
        
        public int? NumericMaxValue { get; set; }
        
        public bool? FutureDateOnly { get; set; }
        
        public bool? PastDateOnly { get; set; }
    }
}