using GLib.Common;
using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface ISystemMessageService : IServiceBase<SystemMessage>
    {
        Task<IEnumerable<SystemMessage>> FindAsync(string id, Paging paging = null);        
    }
}