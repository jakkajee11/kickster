using KickSter.Api.Models;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IClientService : IServiceBase<Client>
    {
        Task<bool> IsExist(string clientId, string domain);        
    }
}