using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Web.Models
{
    public class DatasetInstanceSubValueModel
    {
        public int DatasetElementSubId { get; set; }
        public int InstanceSubValueId { get; set; }
        public string InstanceValue { get; set; }
        public FieldTypes InstanceValueType { get; set; }
        public bool InstanceValueRequired { get; set; }
    }
}