using KickSter.Models;
using System;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Threading.Tasks;
using GLib.Common;
using GLib.Core.Security;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;

namespace KickSter.Api.Services
{

    public class AccountService : ServiceBase<Account>, IAccountService
    {
        //readonly string _collection = AppConfig.AccountCollection;
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Account> _builder;

        public AccountService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Account>.Filter;
            _collection = AppConfig.ACCOUNT_COLLECTION;
        }

        private async Task<string> GetUserSalt(string username)
        {
            var account = await FindAsync("Username", username);

            return account.FirstOrDefault().Salt;
        }

        private async Task<byte[]> GetUserSaltAsBytes(string username)
        {
            var salt = await GetUserSalt(username);

            return Convert.FromBase64String(salt);
        }

        public async Task<string> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return string.Empty;

            var saltBytes = await GetUserSaltAsBytes(username);
            var encryptedPassword = Crypto.ComputeHash(password, AppConfig.HASH_ALGORITHM, saltBytes);
            
            var filter = _builder.Eq(ac => ac.UserName, username) & _builder.Eq(ac => ac.Password, encryptedPassword);
            var document = await _repository.FindAsync(_collection, filter);
            var token = string.Empty;
            var account = document.FirstOrDefault();
            if (account != null)
            {
                token = ObjectId.GenerateNewId().ToString();
                await UpdateTokenAsync(account.Id, token);
            }

            return token;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
                   
            var filter = _builder.Eq("Token", token);
            var result = await _repository.FindAsync(_collection, filter);
            var account = result.FirstOrDefault();

            return (account == null || IsExpired(account.DateTokenExpired)) ? false : true;
        }

        private bool IsExpired(DateTime? dateTokenExpired)
        {
            return (DateTime.Now > dateTokenExpired) ? true : false;
        }

        //public async Task<IEnumerable<Account>> ListAsync(Paging paging = null)
        //{
        //    var findOptions = ServiceHelpers.BuildFindOptions<Account>(paging);
        //    var result = await _repository.FindAsync(_collection, FilterDefinition<Account>.Empty, findOptions);
        //    return result.ToEnumerable();
        //}

        //public async Task<Account> FindOneAsync(string id)
        //{
        //    return await FindOne(ObjectId.Parse(id));
        //}

        //public async Task<Account> FindOne(ObjectId id)
        //{
        //    var filter = _builder.Eq("_id", id);
        //    var result = await _repository.FindAsync(_collection, filter);
        //    return result.FirstOrDefault();
        //}

        

        //public async Task<IEnumerable<Account>> FindAsync(FindOpt find)
        //{
        //    var filter = _builder.Eq(find.Field, find.Value);

        //    var findOptions = ServiceHelpers.BuildFindOptions<Account>(find.Paging);
        //    var result = await _repository.FindAsync(_collection, filter, findOptions);

        //    return result.ToEnumerable();
        //}

        //public async Task<IEnumerable<Account>> FindAsync(string field, string value)
        //{
        //    var filter = _builder.Eq(field, value);
        //    var result = await _repository.FindAsync(_collection, filter);
            
        //    return result.ToEnumerable();
        //}

        

        public override async Task<Account> CreateAsync(Account account)
        {
            var salt = Crypto.GetSaltAsBytes();
            account.Password = Crypto.ComputeHash(account.Password, AppConfig.HASH_ALGORITHM, salt);
            account.Salt = Convert.ToBase64String(salt);
            await _repository.InsertAsync(_collection, account);
            
            return account;
        }        

        public new async Task<Account> UpdateAsync(Account account)
        {
            var oldAccount = await FindOneAsync(account.Id);
            var filter = ServiceHelpers.BuildKeyFilter(_builder, account.Id);
            var now = DateTime.Now;
            var update = Builders<Account>.Update
                        .Set(ac => ac.LastModified, now);
            if (!oldAccount.Email.Equals(account.Email))
            {
                update.Set(ac => ac.Email, account.Email);
            }

            if (!oldAccount.Token.Equals(account.Token))
            {
                update.Set(ac => ac.Email, account.Email);
            }

            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.ModifiedCount == 0) account = null;

            // Update datetime properties
            account.DateCreated = oldAccount.DateCreated;
            account.LastModified = now;                              

            return account;
        }

        //public async Task<Account> ReplaceAsync(Account account)
        //{
        //    var filter = _builder.Eq("_id", account.Id);
        //    var result = await _repository.ReplaceOneAsync(_collection, filter, account);
        //    if (result.ModifiedCount == 0) account = null;
            
        //    return account;
        //}

        public new async Task ImportAsync(IEnumerable<Account> accounts)
        {
            foreach(var account in accounts)
            {
                var salt = Crypto.GetSaltAsBytes();
                account.Password = Crypto.ComputeHash(account.Password, AppConfig.HASH_ALGORITHM, salt);
                account.Salt = Convert.ToBase64String(salt);
            }
            await _repository.InsertManyAsync(_collection, accounts);            
        }

        public async Task<bool> UpdateEmailAsync(string id, string email)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                .Set(ac => ac.Email, email)
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }
            return false;
        }

        public async Task<bool> UpdatePasswordAsync(string id, string password)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                .Set(ac => ac.Password, Crypto.ComputeHash(password, AppConfig.HASH_ALGORITHM, Crypto.GetSaltAsBytes()))
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }
            return false;
        }

        public async Task<bool> UpdateTokenAsync(string id, string token)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                .Set(ac => ac.Token, token)
                .Set(ac => ac.DateTokenExpired, DateTime.Now.AddMinutes(AppConfig.TOKEN_EXPIRED_IN_MINUTES))
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }

            return false;
        }

        public async Task<bool> UpdatePreferenceAsync(string id, Preferences preferences)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                //.Set("Player.Preferences", preferences)
                .Set(ac => ac.Player.Preferences, preferences)
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }

            return false;
        }

        public async Task<bool> UpdateOtherInfoAsync(string id, AdditionalInfo otherInfo)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                //.Set("Player.OtherInfo", otherInfo)
                .Set(ac => ac.Player.OtherInfo, otherInfo)
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }

            return false;
        }

        public async Task<bool> UpdateAvatarAsync(string id, Picture avatar)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Account>.Update
                .Set(ac => ac.Player.Avatar, avatar)
                .CurrentDate(ac => ac.LastModified);
            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.IsModifiedCountAvailable)
            {
                return result.ModifiedCount == 0 ? false : true;
            }

            return false;
        }
    }
}