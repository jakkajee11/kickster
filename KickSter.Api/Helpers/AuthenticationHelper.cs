using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System.Threading.Tasks;

namespace KickSter.Api.Helpers
{
    public class AuthenticationHelper
    {
        private static IUserService _userService;
        private static IClientService _clientService;    
        //private static IMapper _mapper;

        public static void Init(Container container)
        {
            // Simulate WebApi Request
            using (container.BeginExecutionContextScope())
            {
                _userService = container.GetInstance<IUserService>();
                _clientService = container.GetInstance<IClientService>();         
                //_mapper = container.GetInstance<IMapper>();
            }
        }

        public async static Task<UserProfile> FindUser(string token)
        {
            return await _userService.FindOneAsync("Token", token);
        }

        public async static Task<bool> IsClientExist(string clientId, string domain)
        {
            return await _clientService.IsExist(clientId, domain);
        }
    }
}