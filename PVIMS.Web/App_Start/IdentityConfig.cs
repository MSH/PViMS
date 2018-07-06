using System.Threading.Tasks;

using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

using PVIMS.Web.Models;
using PVIMS.Entities.EF;

namespace PVIMS.Web
{
    public class UserInfoManager : UserManager<UserInfo, int>
    {
        public UserInfoManager(IUserStore<UserInfo, int> store, IdentityFactoryOptions<UserInfoManager> options)
            : base(store)
        {
            this.UserValidator = new UserValidator<UserInfo, int>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                this.UserTokenProvider = new DataProtectorTokenProvider<UserInfo, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }
    }
}
