using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetInstanceSubValuesSaveModel
    {
        public int DatasetInstanceId { get; set; }
        public int DatasetElementId { get; set; }
        public Guid SubValueContext { get; set; }
        public InstanceSubValueModel[] Values { get; set; }
    }

    public class InstanceSubValueModel
    {
        public int DatasetElementSubId { get; set; }
        public object Value { get;set; }
    }
}