using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;

namespace PVIMS.Core.Entities
{
    public class DatasetXmlAttribute : AuditedEntityBase
    {
        [Required]
        [StringLength(50)]
        public string AttributeName { get; set; }

        [Required]
        public virtual DatasetXmlNode ParentNode { get; set; }

        public string AttributeValue { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
    }
}