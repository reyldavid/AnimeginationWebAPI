using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using AnimeginationApi.Handlers;
using System.Web.Http.Dispatcher;

namespace AnimeginationApi
{
    public static class WebApiConfig
    {
        private static DelegatingHandler[] AuthenticatingHandlers
        {
            get
            {
                return new DelegatingHandler[] { new ApiKeyHandler() };
            }
        }

        private static DelegatingHandler[] AuthenticatedHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    new ApiKeyHandler(),
                    new JwtTokenHandler()
                };
            }
        }

        private static DelegatingHandler[] AuthenticatedAdminHandlers
        {
            get
            {
                return new DelegatingHandler[]
                {
                    new ApiKeyHandler(),
                    new JwtTokenHandler(),
                    new AdminRoleHandler()
                };
            }
        }

        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new RequestHeaderMapping("Accept", "text/html",
                StringComparison.InvariantCultureIgnoreCase, true,
                "application/json"));

            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
            //    = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var authenticatingHandlers = HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatingHandlers);

            var authenticatedHandlers = HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedHandlers);

            var authenticatedAdminHandlers = HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    AuthenticatedAdminHandlers);

            config.EnableCors();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }, 
                constraints: null,
                handler: authenticatingHandlers
            );

            //config.Routes.MapHttpRoute(
            //    name: "Search",
            //    routeTemplate: "api/{controller}/{search}", 
            //    defaults: new { action = "Search", controller = "Search" }
            //);

        }
    }
}
