using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using System.Web.Http;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;

using WebConfigurationManager = System.Web.Configuration.WebConfigurationManager;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using KickSter.Api.App_Start;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Extensions.ExecutionContextScoping;
using GLib.MongoDB;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository;
using KickSter.Api.Services;
using KickSter.Api.Helpers;
using Swashbuckle.Application;

[assembly: OwinStartup(typeof(KickSter.Api.Startup))]
namespace KickSter.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Auth0 
            var issuer = AppConfig.AUTH0_DOMAIN;// WebConfigurationManager.AppSettings["Auth0Domain"];
            var audience = AppConfig.AUTH0_CLIENT_ID;// WebConfigurationManager.AppSettings["Auth0ClientID"];
            var secret = TextEncodings.Base64Url.Decode(AppConfig.AUTH0_CLIENT_SECRET); //(WebConfigurationManager.AppSettings["Auth0ClientSecret"]);


            app.UseCors(CorsOptions.AllowAll);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    },
                });

            var container = SimpleInjectorInitializer.Initialize(app);
            
            //// Create the container as usual.
            //var container = new Container();
            //// Register your types, for instance using the RegisterWebApiRequest
            //// extension from the integration package:       

            //container.RegisterWebApiRequest<IMongoConnector>(() => new MongoConnector(AppConfig.ConnectionString));
            //container.RegisterWebApiRequest<IRepository, MongoDBRepository>();
            //container.RegisterWebApiRequest<IAccountService, AccountService>();
            //container.RegisterWebApiRequest<IUserService, UserService>();
            //container.RegisterWebApiRequest<IArenaService, ArenaService>();
            //container.RegisterWebApiRequest<IGroupService, GroupService>();
            //container.RegisterWebApiRequest<IPostService, PostService>();
            //container.RegisterWebApiRequest<ITeamService, TeamService>();
            //container.RegisterWebApiRequest<IZoneService, ZoneService>();
            //container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            //container.Verify();

            //GlobalConfiguration.Configuration.DependencyResolver =
            //      new SimpleInjectorWebApiDependencyResolver(container);

            //var config = new HttpConfiguration();
            HttpConfiguration config = new HttpConfiguration
            {
                DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container)
            };

            // EnableSwagger (api document http://localhost:8000/swagger/)
            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "Kickster API"))
                .EnableSwaggerUi();



            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}