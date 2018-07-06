using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class UserFacility : EntityBase
    {
        public User User { get; set; }
        public Facility Facility { get; set; }
    }
}
