using AutoMapper;
using GLib.MongoDB;
using KickSter.Api.Helpers;
using KickSter.Api.Repository;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Services;
using KickSter.Api.Services.Interfaces;
using Owin;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Linq;
using System.Web.Http;

namespace KickSter.Api.App_Start
{


    public static class SimpleInjectorInitializer
    {
        public static Container Initialize(IAppBuilder app)
        {
            var container = GetInitializeContainer(app);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                  new SimpleInjectorWebApiDependencyResolver(container);            

            return container;
        }

        public static Container GetInitializeContainer(IAppBuilder app)
        {
            // 1. Create a new Simple Injector container
            var container = new Container();

            container.RegisterSingleton(app);
            container.Options.AllowOverridingRegistrations = true;
            // 2. Configure the container (register)
            // AutoMapper
            container.RegisterSingleton(AutoMapperInitializer.Configure);
            // Register Services
            container.RegisterSingleton<IMongoConnector>(() => new MongoConnector(AppConfig.CONNECTION_STRING));
            container.RegisterWebApiRequest<IRepository, MongoDBRepository>();
            container.RegisterWebApiRequest<IAccountService, AccountService>();
            container.RegisterWebApiRequest<IUserService, UserService>();
            container.RegisterWebApiRequest<IArenaService, ArenaService>();
            container.RegisterWebApiRequest<IGroupService, GroupService>();
            container.RegisterWebApiRequest<IPostService, PostService>();
            container.RegisterWebApiRequest<ITeamService, TeamService>();
            container.RegisterWebApiRequest<IZoneService, ZoneService>();
            container.RegisterWebApiRequest<IMessageService, MessageService>();
            container.RegisterWebApiRequest<IPreferenceDayService, PreferenceDayService>();
            container.RegisterWebApiRequest<IClientService, ClientService>();
            container.RegisterWebApiRequest<IFeedbackService, FeedbackService>();
            container.RegisterWebApiRequest<ISystemMessageService, SystemMessageService>();
            container.RegisterWebApiRequest<INewsService, NewsService>();
            //RegisterInterface(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            // Initialize Static Helper classes
            MatchingHelpers.Init(container);
            AuthenticationHelper.Init(container);
            // 3. Optionally verify the container's configuration.
            container.Verify();

            // 4. Store the container for use by the application
            GlobalConfiguration.Configuration.DependencyResolver =
                    new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }      
        
        private static void RegisterInterface(Container container)
        {
            var ns = "KickSter.Api";
            var repositoryAssembly = typeof(MongoDBRepository).Assembly;

            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace.StartsWith(ns)
                where type.GetInterfaces().Where(t => !t.IsGenericType && (t.FullName.StartsWith(ns + ".Repository") || t.FullName.StartsWith(ns + ".Services"))).Any()
                select new { Service = type.GetInterfaces().First(), Implementation = type };

            foreach (var reg in registrations)
            {
                container.Register(reg.Service, reg.Implementation, Lifestyle.Transient);
            }
        }  
    }
}