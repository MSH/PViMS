using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;

namespace PVIMS.Core.Entities
{
    public class UserRole : Entity<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
