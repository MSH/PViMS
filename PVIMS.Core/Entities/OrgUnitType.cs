using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class OrgUnitType : EntityBase
    {
        public OrgUnitType()
        {
        }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        public virtual OrgUnitType Parent { get; set; }

        [InverseProperty("Parent")]
        public virtual ICollection<OrgUnitType> Children { get; set; }


    }
}