using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using PVIMS.Core.ValueTypes;
using PVIMS.Core.Exceptions;

namespace PVIMS.Core.Entities
{
    public class DatasetXmlNode : AuditedEntityBase
    {
        public DatasetXmlNode()
        {
            ChildrenNodes = new HashSet<DatasetXmlNode>();
            NodeAttributes = new HashSet<DatasetXmlAttribute>();
        }

        [Required]
        [StringLength(50)]
        public string NodeName { get; set; }
        [Required]
        public NodeType NodeType { get; set; }

        public virtual DatasetXmlNode ParentNode { get; set; }
        public string NodeValue { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
        public virtual DatasetElementSub DatasetElementSub { get; set; }

        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }
        public virtual ICollection<DatasetXmlAttribute> NodeAttributes { get; set; }
   }
}