using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PVIMS.Web.Models
{
    public class CustomAttributeAddModel
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        [HiddenInput(DisplayValue = false)]
        public bool IsRequired { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int? StringMaxLength { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int? NumericMinValue { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int? NumericMaxValue { get; set; }
        [HiddenInput(DisplayValue = false)]
        public bool PastDateOnly { get; set; }
        [HiddenInput(DisplayValue = false)]
        public bool FutureDateOnly { get; set; }
    }
}