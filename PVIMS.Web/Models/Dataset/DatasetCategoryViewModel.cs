using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PVIMS.Web.Models
{
    public class DatasetCategoryViewModel
    {
        [Key]
        public long DatasetCategoryId { get; set; }
        [Display(Name = "Category Name")]
        public string DatasetCategoryName { get; set; }
        public bool DatasetCategoryDisplayed { get; set; }
        public DatasetElementViewModel[] DatasetElements { get; set; }
    }
}