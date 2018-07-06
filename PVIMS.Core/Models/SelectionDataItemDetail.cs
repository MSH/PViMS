using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PVIMS.Core.Models
{
    public class SelectionDataItemDetail
    {
        public long SelectionDataItemId { get; set; }
        [DisplayName("Attribute")]
        public string AttributeKey { get; set; }
        [Required]
        [DisplayName("Display Text")]
        public string DataItemValue { get; set; }
        [Required]
        [DisplayName("Key")]
        public string SelectionKey { get; set; }
    }
}
