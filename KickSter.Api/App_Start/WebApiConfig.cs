using KickSter.Api.App_Start;
using KickSter.Api.Filters;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KickSter.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Add global authentication filters
            //config.Filters.Add(new CustomAuthenticationFilterAttribute());
            config.MessageHandlers.Add(new JsonWebTokenValidationHandler
            {
                Audience = AppConfig.AUTH0_CLIENT_ID,
                SymmetricKey = AppConfig.AUTH0_CLIENT_SECRET
            });
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
