using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;

namespace PVIMS.Core.Entities
{
    public class DatasetXml : AuditedEntityBase
    {
        public DatasetXml()
        {
            ChildrenNodes = new HashSet<DatasetXmlNode>();
        }

        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }

    }
}