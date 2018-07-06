using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    public class OrgUnit : EntityBase
    {
        public OrgUnit()
        {
        }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual OrgUnit Parent { get; set; }

        [Required]
        public virtual OrgUnitType OrgUnitType { get; set; }

        [InverseProperty("Parent")]
        public virtual ICollection<OrgUnit> Children { get; set; }

    }
}