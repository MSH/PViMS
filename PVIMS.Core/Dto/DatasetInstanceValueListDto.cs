using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using PVIMS.Core.Entities;

namespace PVIMS.Core.Dto
{
    public class DatasetInstanceValueListDto
    {
        public DatasetInstanceValueListDto()
        {
            Values = new HashSet<DatasetInstanceValueListItem>();
        }

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
