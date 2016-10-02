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

    public class UserService : ServiceBase<UserProfile>, IUserService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<UserProfile> _builder;
        private readonly FindOneAndUpdateOptions<UserProfile> _findOneUpdateOptions;
        //private UpdateOptions _updateOptions;
        public UserService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<UserProfile>.Filter;
            _collection = AppConfig.USER_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<UserProfile>(true);
        }

        private async Task<string> GetUserSalt(string username)
        {
            var user = await FindAsync("Username", username);

            return user.FirstOrDefault().Salt;
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
            var user = document.FirstOrDefault();
            //if (user != null)
            //{
            //    token = ObjectId.GenerateNewId().ToString();
            //    await UpdateTokenAsync(user.Id, token);
            //}

            //return token;
            return ObjectId.GenerateNewId().ToString();
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
                   
            var filter = _builder.Eq("Token", token);
            var result = await _repository.FindAsync(_collection, filter);
            var user = result.FirstOrDefault();

            return (user == null || IsExpired(user.DateTokenExpired)) ? false : true;
        }

        private bool IsExpired(DateTime? dateTokenExpired)
        {
            return (DateTime.Now > dateTokenExpired) ? true : false;
        }

        public override async Task<UserProfile> CreateAsync(UserProfile user)
        {
            var filter = _builder.Or(_builder.Eq(up => up.Id, user.Id), _builder.Eq(up => up.NickName, user.NickName), _builder.Eq(up => up.Email, user.Email));
            var oldUser = await FindAsync(_collection, filter);
            // Throw exception if username already in used.
            if (oldUser.Count() > 0)
            {
                if (!string.IsNullOrEmpty(user.Id))
                    throw new ApplicationException("User already exist.");
                else if (!string.IsNullOrEmpty(user.Email))
                    throw new ApplicationException("Email is being used by another user.");
                else
                    user.NickName = user.NickName + ObjectId.GenerateNewId().ToString();
            }
            // Only Internal user has password
            if (!string.IsNullOrEmpty(user.UserType) && user.UserType.ToLower().Equals("internal"))
            {
                var salt = Crypto.GetSaltAsBytes();
                user.Password = Crypto.ComputeHash(user.Password, AppConfig.HASH_ALGORITHM, salt);
                user.Salt = Convert.ToBase64String(salt);
            }
            //if (user.Preferences == null)
            //    user.Preferences = new Preferences();
            // Set datetime of creation to Now if it not set by client
            user.DateCreated = DateTime.MinValue == user.DateCreated ? DateTime.Now : user.DateCreated;
            user.LastLogin = DateTime.Now;
            user.TotalPost = 0;
            await _repository.InsertAsync(_collection, user);

            user.LastLogin = null;
            return user;
        }        

        public override async Task<UserProfile> UpdateAsync(UserProfile user)
        {
            var filter = _builder.Eq(up => up.Id, user.Id);
            var now = DateTime.Now;
            var update = Builders<UserProfile>.Update                        
                        .Set(up => up.Picture, user.Picture)
                        .Set(up => up.Email, user.Email)
                        .Set(up => up.NickName, user.NickName)
                        .Set(up => up.Gender, user.Gender)                                                                   
                        .Set(up => up.LastModified, now);

            if (!string.IsNullOrEmpty(user.LastIp))
                update = update.Set(up => up.LastIp, user.LastIp);
            if (!string.IsNullOrEmpty(user.Phone))
                update = update.Set(up => up.Phone, user.Phone);
            if (user.LastLogin.HasValue)
                update = update.Set(up => up.LastLogin, user.LastLogin.Value);
            if (user.BirthDate.HasValue)
                update = update.Set(up => up.BirthDate, user.BirthDate.Value);
            if (user.Social != null && (!string.IsNullOrEmpty(user.Social.Facebook) || !string.IsNullOrEmpty(user.Social.Line)))
                update = update.Set(up => up.Social, user.Social);
            if (user.Preferences != null)
                update = update.Set(up => up.Preferences, user.Preferences);
            if (user.IsPublic.HasValue)
                update = update.Set(up => up.IsPublic, user.IsPublic.Value);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public override async Task ImportAsync(IEnumerable<UserProfile> users)
        {            
            var existUsers = await FindInAsync("UserName", users.Select(u => u.UserName));
            var newUsers = users.Where(u => !existUsers.ToList().Exists(eu => eu.UserName.Equals(u.UserName, StringComparison.OrdinalIgnoreCase)));
            foreach (var user in newUsers)
            {
                var salt = Crypto.GetSaltAsBytes();
                user.Password = Crypto.ComputeHash(user.Password, AppConfig.HASH_ALGORITHM, salt);
                user.Salt = Convert.ToBase64String(salt);
            }

            await _repository.InsertManyAsync(_collection, newUsers);            
        }

        public async Task<UserProfile> UpdateEmailAsync(string id, string email)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Set(ac => ac.Email, email)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> UpdatePasswordAsync(string id, string password)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Set(ac => ac.Password, Crypto.ComputeHash(password, AppConfig.HASH_ALGORITHM, Crypto.GetSaltAsBytes()))
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update);
            
        }

        public async Task<UserProfile> UpdateTokenAsync(string id, string token)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Set(ac => ac.Token, token)
                .Set(ac => ac.DateTokenExpired, DateTime.Now.AddMinutes(AppConfig.TOKEN_EXPIRED_IN_MINUTES))
                .CurrentDate(ac => ac.LastModified);
            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> UpdatePreferenceAsync(string id, Preferences preferences)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Set(up => up.Preferences, preferences)
                .CurrentDate(ac => ac.LastModified);
            //var options = ServiceHelpers.BuildFindOneAndUpdateOptions<UserProfile>(true);
            
            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);            
        }

        //public async Task<bool> UpdateOtherInfoAsync(string id, AdditionalInfo otherInfo)
        //{
        //    var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
        //    var update = Builders<UserProfile>.Update
        //        //.Set("Player.OtherInfo", otherInfo)
        //        .Set(ac => ac.OtherInfo, otherInfo)
        //        .CurrentDate(ac => ac.LastModified);
        //    var result = await _repository.UpdateOneAsync(_collection, filter, update);
        //    if (result.IsModifiedCountAvailable)
        //    {
        //        return result.ModifiedCount == 0 ? false : true;
        //    }

        //    return false;
        //}

        public async Task<UserProfile> UpdateAvatarAsync(string id, string avatar)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Set(ac => ac.Picture, avatar)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> GetUserProfile(string username)
        {
            var filter = _builder.Eq(u => u.UserName, username);
            return await _repository.FindOneAsync(_collection, filter);
        }

        public async Task<UserProfile> AddZonePreferenceAsync(string id, List<ZoneBase> zones)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .PushEach(up => up.Preferences.Zones, zones)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> AddArenaPreferenceAsync(string id, List<Arena> arenas)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .PushEach(up => up.Preferences.Arenas, arenas)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> AddDayPreferenceAsync(string id, List<DayTimes> days)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .PushEach(up => up.Preferences.DayTimes, days)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> RemoveZonePreferenceAsync(string id, string zoneId)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var pullFilter = _builder.AnyEq("Id", ObjectId.Parse(zoneId));
            var update = Builders<UserProfile>.Update
                .PullFilter("Preference.Zones", pullFilter)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> RemoveArenaPreferenceAsync(string id, string arenaId)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var pullFilter = _builder.AnyEq("Id", ObjectId.Parse(arenaId));
            var update = Builders<UserProfile>.Update
                .PullFilter("Preference.Arenas", pullFilter)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<UserProfile> RemoveDayPreferenceAsync(string id, string day, string time)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var pullFilter = _builder.AnyEq("Day", day) & _builder.AnyEq("Time", time);
            var update = Builders<UserProfile>.Update
                .PullFilter("Preference.DayTimes", pullFilter)
                .CurrentDate(ac => ac.LastModified);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        //private UserProfile Find(string field, string value)
        //{
        //    FindOneAsync()
        //    var filter = _builder.Eq(field, value);
        //    return await _repository.FindOneAsync(_collection, filter);
        //}

        public async Task<UserProfile> Auth0SyncUserAsync(UserProfile user)
        {
            try
            {
                return await CreateAsync(user);
            } catch (ApplicationException)
            {
                return await UpdateAsync(user);
            }                      
        }

        public async Task<UserProfile> GetUserById(string id)
        {
            return await FindOneAsync("_id", id);
        }

        public async Task<UserProfile> UpdateTotalPost(string id)
        {
            var filter = ServiceHelpers.BuildStringKeyFilter(_builder, id);
            var update = Builders<UserProfile>.Update
                .Inc(ac => ac.TotalPost, 1);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<bool> IsNameInUsed(string id, string nickname)
        {
            var users = await FindAsync("NickName", nickname);
            return users.Where(u => !u.Id.Equals(id)).Count() > 0 ? true : false;
        }
    }
}