using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Data;
using MvcCms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcCms.App_Start
{
    public class AuthDbConfig
    {
        public async static Task RegisterAdmin()
        {
            await CreateAdminUser();


            using (var roles = new RoleRepository())
            {
                await CreateRoleIfNotExist(roles, "admin");
                await CreateRoleIfNotExist(roles, "editor");
                await CreateRoleIfNotExist(roles, "author");
            }
        }

        private static async Task CreateAdminUser()
        {
            using (var users = new UserRepository())
            {
                var user = await users.GetUserByNameAsync("admin");
                if (user != null) return;

                var adminUser = new CmsUser
                {
                    UserName = "admin",
                    Email = "admin@cms.com",
                    DisplayName = "Administrator"
                };

                await users.CreateAsync(adminUser, "Passw0rd1234");
            }
        }

        private static async Task CreateRoleIfNotExist(RoleRepository roles, string role)
        {
            if (await roles.GetRoleByNameAsync(role) == null)
            {
                await roles.CreateAsync(new IdentityRole(role));
            }
        }
    }
}