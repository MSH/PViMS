using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using VPS.Common.Repositories;
using PVIMS.Core.Entities;
using PVIMS.Web.Models;

namespace PVIMS.Web
{
    public class UserInfoStore : IUserStore<UserInfo, int>,
        IUserPasswordStore<UserInfo, int>,
        IUserEmailStore<UserInfo, int>,
        IUserRoleStore<UserInfo, int>
    {
        private readonly IUnitOfWorkInt unitOfWork;
        private readonly IRepositoryInt<UserRole> userRoleRepository;
        private readonly IRepositoryInt<Role> roleRepository;
        private readonly IRepositoryInt<User> userRepository;

        public UserInfoStore(IUnitOfWorkInt unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.userRoleRepository = unitOfWork.Repository<UserRole>();
            this.roleRepository = unitOfWork.Repository<Role>();
            this.userRepository = unitOfWork.Repository<User>();
        }

        public async Task CreateAsync(UserInfo userInfo)
        {
            var user = new User
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                UserName = userInfo.UserName,
                PasswordHash = userInfo.PasswordHash,
                Email = userInfo.Email
            };

            await Task.Run(() =>
            {
                userRepository.Save(user);
                userInfo.Id = user.Id;
            });
        }

        public async Task DeleteAsync(UserInfo user)
        {
            throw new NotImplementedException("Users should be deleted via the UserAdministration Interface");
        }

        public async Task<UserInfo> FindByIdAsync(int userId)
        {
            var user = await userRepository
                .Queryable()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var userInfo = new UserInfo { Id = user.Id, UserName = user.UserName, PasswordHash = user.PasswordHash, Email = user.Email };

            return userInfo;
        }

        public async Task<UserInfo> FindByNameAsync(string userName)
        {
            var user = await userRepository
                .Queryable()
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return null;

            var userInfo = new UserInfo { Id = user.Id, UserName = user.UserName, PasswordHash = user.PasswordHash };

            return userInfo;
        }

        public async Task UpdateAsync(UserInfo user)
        {
            // Should throw a not implemented exception here and indicate that the user must be updated via the UserAdmistrationController but UserManager.AddToRoleAsync makes a call to this method internally.
            return;
        }

        public async Task AddLoginAsync(UserInfo user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInfo> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveLoginAsync(UserInfo user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<string> GetPasswordHashAsync(UserInfo userInfo)
        {
            return await Task<string>.Run(() => userInfo.PasswordHash);
        }

        public async Task<bool> HasPasswordAsync(UserInfo user)
        {
            return await Task<bool>.Run(() => !string.IsNullOrEmpty(user.PasswordHash));
        }

        public async Task SetPasswordHashAsync(UserInfo userInfo, string passwordHash)
        {
            await Task.Run(() => userInfo.PasswordHash = passwordHash);
        }

        public async Task<UserInfo> FindByEmailAsync(string email)
        {
            var user = await unitOfWork.Repository<User>()
                .Queryable()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            return new UserInfo { Id = user.Id, Email = user.Email, UserName = user.UserName };
        }

        public async Task<string> GetEmailAsync(UserInfo userInfo)
        {
            return await Task<string>.Run(() => userInfo.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(UserInfo user)
        {
            return true;
        }

        public async Task SetEmailAsync(UserInfo userInfo, string email)
        {
            await Task.Run(() => userInfo.Email = email);
        }

        public async Task SetEmailConfirmedAsync(UserInfo user, bool confirmed)
        {
            return;
        }

        public async Task AddToRoleAsync(UserInfo userInfo, string roleName)
        {
            var user = userRepository.Get(userInfo.Id);
            var role = roleRepository.Queryable().SingleOrDefault(r => r.Key == roleName);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (role == null)
            {
                throw new InvalidOperationException("Role not recognised.");
            }

            await Task.Run(() =>
            {
                userRoleRepository.Save(new UserRole { User = user, Role = role });
            });
        }

        public async Task<IList<string>> GetRolesAsync(UserInfo userInfo)
        {
            return await userRoleRepository.Queryable()
                .Where(ur => ur.User.Id == userInfo.Id)
                .Select(i => i.Role.Key)
                .ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(UserInfo userInfo, string roleName)
        {
            return await userRoleRepository.Queryable().AnyAsync(ur => ur.User.Id == userInfo.Id && ur.Role.Key == roleName);
        }

        public Task RemoveFromRoleAsync(UserInfo user, string roleName)
        {
            throw new NotImplementedException();
        }
    }
}