using System.Security.Principal;
using PVIMS.Core.Entities;

namespace PVIMS.Core
{
    public class PrimaryPrincipal : GenericPrincipal
    {
        public PrimaryPrincipal(User user, IIdentity identity, string[] roles)
            : base(identity, roles)
        {
            this.User = user;

        }

        public User User { get; private set; }
    }
}