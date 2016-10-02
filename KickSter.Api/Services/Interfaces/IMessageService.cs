using GLib.Common;
using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IMessageService : IServiceBase<UserMessage>
    {
        Task<IEnumerable<UserMessage>> FindAsync(string id, Paging paging = null);
        Task<UserMessage> AddMessageAsync(string id, IEnumerable<Message> messages);
    }
}