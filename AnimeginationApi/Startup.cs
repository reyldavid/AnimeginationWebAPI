using AnimeginationApi.App_Start;
using AnimeginationApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using AnimeginationApi.Services;

[assembly: OwinStartupAttribute(typeof(AnimeginationApi.Startup))]
namespace AnimeginationApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureWebApi(httpConfig);

            ConfigureRolesAndUsers(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);
        }

        private void ConfigureRolesAndUsers(IAppBuilder app)
        {
            RolesManager.InitializeUserRoles();

            ////var roleM = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<AnimeDB>()));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(AnimeDB.Create()));

            //if (!roleManager.RoleExists("Admin"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Admin";
            //    roleManager.Create(role);                
            //}

            //if (!roleManager.RoleExists("Developer"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Developer";
            //    roleManager.Create(role);
            //}

            //if (!roleManager.RoleExists("Customer"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Customer";
            //    roleManager.Create(role);
            //}

            //if (!roleManager.RoleExists("Guest"))
            //{
            //    var role = new IdentityRole();
            //    role.Name = "Guest";
            //    roleManager.Create(role);
            //}
        }

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(AnimeDB.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Plugin the OAuth bearer JSON Web Token tokens generation and Consumption will be here

        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}

