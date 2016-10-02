using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IGroupService : IServiceBase<Group>
    {
        Task<bool> AddTeams(string id, IEnumerable<Team> teams);
        Task<bool> RemoveTeam(string id, string teamId);
    }
}