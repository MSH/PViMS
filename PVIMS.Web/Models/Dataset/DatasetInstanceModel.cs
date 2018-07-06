using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetInstanceModel
    {
        public DatasetCategoryEditModel[] DatasetCategories { get; set; }
        public int DatasetInstanceId { get; set; }
    }
}