using GLib.Common;
using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface ISystemService //: IServiceBase<SystemMessage>
    {
        Task InitializeAsync();        
    }
}