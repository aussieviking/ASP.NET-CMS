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
            //using (var context = new CmsContext("LocalDb"))
            //using (var userStore = new UserStore<CmsUser>(context))
            using (var users = new UserRepository())
            {
                var user = users.GetUserByName("admin");
                if (user != null) return;

                var adminUser = new CmsUser
                {
                    UserName = "admin",
                    Email = "admin@cms.com",
                    DisplayName = "Administrator"
                };

                await users.CreateAsync(adminUser, "Passw0rd1234");
            }

            using (var roles = new RoleRepository())
            {
                CreateRoleIfNotExist(roles, "admin");
                CreateRoleIfNotExist(roles, "editor");
                CreateRoleIfNotExist(roles, "author");
            }
        }

        private static void CreateRoleIfNotExist(RoleRepository roles, string role)
        {
            if (roles.GetRoleByName(role) == null)
            {
                roles.Create(new IdentityRole(role));
            }
        }
    }
}