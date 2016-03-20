using System;
using System.Collections;
using System.Collections.Generic;
using MvcCms.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace MvcCms.Data
{
    public interface IUserRepository : IDisposable
    {
        Task<CmsUser> GetUserByNameAsync(string username);
        Task<IEnumerable<CmsUser>> GetAllUsersAsync();
        Task CreateAsync(CmsUser user, string password);
        Task DeleteAsync(CmsUser user);
        Task UpdateAsync(CmsUser user);
        PasswordVerificationResult VerifyHashedPassword(string passwordHash, string currentPassword);
        string HashPassword(string password);
        Task AddUserToRoleAsync(CmsUser user, string role);
        Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user);
        Task RemoveUserFromRolesAsync(CmsUser user, params string[] roleNames);
        Task<CmsUser> GetLoginUserAsync(string username, string password);
        Task<ClaimsIdentity> CreateIdentityAsync(CmsUser user);
    }
}
