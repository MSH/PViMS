using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

using PVIMS.Web.Providers;
using PVIMS.Web.Models;

using Microsoft.AspNet.Identity.Owin;

namespace PVIMS.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = CreateCookieAuthenticationProvider()
            });
        }

        private CookieAuthenticationProvider CreateCookieAuthenticationProvider()
        {
            return new CookieAuthenticationProvider
            {
                OnValidateIdentity = SecurityStampValidator
                    .OnValidateIdentity(
                        TimeSpan.FromMinutes(30),
                        RegenerateIdentityCallback(),
                        GetUserIdCallback)
            };
        }

        private static Func<UserInfoManager, UserInfo, Task<ClaimsIdentity>> RegenerateIdentityCallback()
        {
            return (manager, user) => user.GenerateUserIdentityAsync(manager);
        }

        private int GetUserIdCallback(ClaimsIdentity i)
        {
            return i.GetUserId<int>();
        }
    }
}
