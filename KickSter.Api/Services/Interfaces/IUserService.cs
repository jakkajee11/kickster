using GLib.Common;
using KickSter.Api.Models;
using KickSter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IUserService : IServiceBase<UserProfile>
    {
        Task<UserProfile> GetUserProfile(string username);
        Task<UserProfile> GetUserById(string id);
        Task<string> Login(string username, string password);
        Task<bool> ValidateTokenAsync(string token);        
        Task<UserProfile> UpdateEmailAsync(string id, string email);
        Task<UserProfile> UpdatePasswordAsync(string id, string password);
        Task<UserProfile> UpdateTokenAsync(string id, string token);
        Task<UserProfile> UpdatePreferenceAsync(string id, Preferences preferences);
        Task<UserProfile> AddZonePreferenceAsync(string id, List<ZoneBase> zones);
        Task<UserProfile> AddArenaPreferenceAsync(string id, List<Arena> arenas);
        Task<UserProfile> AddDayPreferenceAsync(string id, List<DayTimes> days);
        Task<UserProfile> RemoveZonePreferenceAsync(string id, string zoneId);
        Task<UserProfile> RemoveArenaPreferenceAsync(string id, string arenaId);
        Task<UserProfile> RemoveDayPreferenceAsync(string id, string day, string time);
        //Task<bool> UpdateOtherInfoAsync(string id, AdditionalInfo otherInfo);
        Task<UserProfile> UpdateAvatarAsync(string id, string avatar);
        Task<UserProfile> UpdateTotalPost(string id);
        Task<bool> IsNameInUsed(string id, string nickname);

        Task<UserProfile> Auth0SyncUserAsync(UserProfile user);
    }
}