using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using PVIMS.Core.Entities;

namespace PVIMS.Core.Models
{
    public class DatasetInstanceValueList
    {
        public DatasetInstanceValueList()
        {
            Values = new HashSet<DatasetInstanceValueListItem>();
        }

        [Required]
        public DatasetElement DatasetElement { get; set; }

        public virtual ICollection<DatasetInstanceValueListItem> Values { get; set; }
    }

    public class DatasetInstanceValueListItem
    {
        [Required]
        [DisplayName("Value Date")]
        public DateTime ValueDate { get; set; }
        [Required]
        public String Value { get; set; }
    }

}
