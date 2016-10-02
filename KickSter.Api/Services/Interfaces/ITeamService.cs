using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface ITeamService : IServiceBase<Team>
    {
        Task<Team> FindTeamByUser(string userId);
        Task<bool> AddMemberAsync(string id, IEnumerable<Member> members);
        Task<bool> RemoveMemberAsync(string id, IEnumerable<Member> members);
        Task<bool> BlockMemberAsync(string id, IEnumerable<Member> members);
        Task<bool> RequestMemberAsync(string id, IEnumerable<Member> members);
    }
}