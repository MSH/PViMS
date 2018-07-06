using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages.Html;

namespace VPS.CustomAttributes
{
    // MIKE: 1-1 between DTO and Attribute
    public class CustomAttributeDetail
    {
        public string AttributeKey { get; set; }
        public string Category { get; set; }
        public CustomAttributeType Type { get; set; }
        public object Value { get; set; }
        public List<SelectListItem> RefData { get; set; }
    }
}
