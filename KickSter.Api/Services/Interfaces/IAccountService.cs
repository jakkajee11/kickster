using GLib.Common;
using KickSter.Api.Models;
using KickSter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IAccountService : IServiceBase<Account>
    {
        Task<string> Login(string username, string password);
        Task<bool> ValidateTokenAsync(string token);
        
        //Task<IEnumerable<Account>> ListAsync(Paging paging = null);
        //Task<Account> FindOneAsync(string id);
        //Task<IEnumerable<Account>> FindAsync<T>(FindOpt<T> find);
        //Task<List<Account>> FindAsync(FindOpt<int> find);
        //Task<IEnumerable<Account>> FindAsync(string field, string value);
        //List<Account> Search(Dictionary<string, object> keyValues);

        //Task<Account> FindOneAsync(string id);
        //Task<Account> CreateAsync(Account account);
        //Task<Account> UpdateAsync(Account account);
        //Task<Account> ReplaceAsync(Account account);
        //Task ImportAsync(IEnumerable<Account> accounts);

        Task<bool> UpdateEmailAsync(string id, string email);
        Task<bool> UpdatePasswordAsync(string id, string password);
        Task<bool> UpdateTokenAsync(string id, string token);
        Task<bool> UpdatePreferenceAsync(string id, Preferences preferences);
        Task<bool> UpdateOtherInfoAsync(string id, AdditionalInfo otherInfo);
        Task<bool> UpdateAvatarAsync(string id, Picture avatar);
    }
}