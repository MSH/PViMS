using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Web.Models
{
    public class DatasetInstanceSubValueGroupingModel
    {
        public Guid Context { get; set; }
        public DatasetInstanceSubValueModel[] Values { get; set; }
    }
}