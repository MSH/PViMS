using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetElementSubEditModel
    {
        public int DatasetElementSubId { get; set; }
        public string DatasetElementSubName { get; set; }
        public string DatasetElementSubDisplayName { get; set; }
        public string DatasetElementSubHelp { get; set; }
        public bool DatasetElementSubRequired { get; set; }
        public string DatasetElementSubType { get; set; }
        public string DatasetElementSubValue { get; set; }
    }
}