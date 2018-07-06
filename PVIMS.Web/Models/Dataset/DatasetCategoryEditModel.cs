using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetCategoryEditModel
    {
        public long DatasetCategoryId { get; set; }
        public string DatasetCategoryDisplayName { get; set; }
        public string DatasetCategoryHelp { get; set; }
        public bool DatasetCategoryDisplayed { get; set; }
        public DatasetElementEditModel[] DatasetElements { get; set; }
    }
}