using System.Web.Mvc;

namespace PVIMS.Web.Models
{
    public class CustomAttributeEditModel
    {
        public CustomAttributeEditModel()
        {
            IsRequired = false;
            PastDateOnly = false;
            FutureDateOnly = false;
            AllowModal = false;
        }

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
        public bool AllowModal { get; set; }
    }
}