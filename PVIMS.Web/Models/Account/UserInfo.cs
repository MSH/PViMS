using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

using PVIMS.Core.Entities;

namespace PVIMS.Web.Models
{
    public class UserInfo : IdentityUser<int, IdentityUserLogin<int>, UserRoleInfo, IdentityUserClaim<int>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserInfo, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    // NOTE: Can possibly remove the below classes.
    public class RoleInfo : IdentityRole<int, IdentityUserRole<int>>
    {
        public string RoleKey { get; set; }
    }

    public class UserRoleInfo : IdentityUserRole<int>
    {
    }
}