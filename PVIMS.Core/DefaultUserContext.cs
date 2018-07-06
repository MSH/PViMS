using System.Threading;
using PVIMS.Core.Entities;

namespace PVIMS.Core
{
    public class DefaultUserContext : UserContext
    {
        public override User User
        {
            get
            {
                var principal = Thread.CurrentPrincipal as PrimaryPrincipal;

                if (principal == null)
                {
                    return null;
                }

                return principal.User;
            }
        }

        public override bool IsInRole(string role)
        {
            var principal = Thread.CurrentPrincipal;

            if (principal == null)
            {
                return false;
            }

            return principal.IsInRole(role);
        }
    }
}
