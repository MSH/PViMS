using System.Linq;
using System.Threading;
using System.Web;

using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public class HttpContextUserContext : UserContext
    {
        public override User User
        {
            get
            {
                var principal = Thread.CurrentPrincipal as PrimaryPrincipal;

                if (principal != null)
                {
                    return principal.User;
                }

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var userName = HttpContext.Current.User.Identity.Name;

                    var db = new PVIMSDbContext();
                    var user = db.Users.Single(u => u.UserName == userName);

                    Thread.CurrentPrincipal = new PrimaryPrincipal(user, HttpContext.Current.User.Identity, new string[] { });
                    return user;
                }

                return null;
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