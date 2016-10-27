using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimeginationApi.Models;

namespace AnimeginationApi.Services
{
    public class RolesManager
    {
        public static void InitializeUserRoles()
        {
            //var roleM = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<AnimeDB>()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(AnimeDB.Create()));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Developer"))
            {
                var role = new IdentityRole();
                role.Name = "Developer";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Customer"))
            {
                var role = new IdentityRole();
                role.Name = "Customer";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Guest"))
            {
                var role = new IdentityRole();
                role.Name = "Guest";
                roleManager.Create(role);
            }
        }

        public static string[] UserRolesNames(List<IdentityUserRole> roles)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(AnimeDB.Create()));

            List<string> rolesList = new List<string>();

            roles.ToList().ForEach(r => rolesList.Add(roleManager.FindById(r.RoleId).Name));

            return rolesList.ToArray();
        }
    }
}