using MongoDB.Driver;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;

namespace KickSter.Api.Services
{

    public class TeamService : ServiceBase<Team>, ITeamService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Team> _builder;

        public TeamService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Team>.Filter;
            _collection = AppConfig.TEAM_COLLECTION;
        }

        public override async Task<Team> CreateAsync(Team team)
        {
            if (team.Members == null) team.Members = new List<Member>();
            if (team.RequestPending == null) team.RequestPending = new List<Member>();
            if (team.BlockedMembers == null) team.BlockedMembers = new List<Member>();
            if (team.DateCreated == DateTime.MinValue) team.DateCreated = DateTime.Now;
            return await base.CreateAsync(team);
        }

        public async Task<bool> AddMemberAsync(string id, IEnumerable<Member> members)
        {
            var team = await FindOneAsync(id);

            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Team>.Update
                        .Set(doc => doc.LastModified, DateTime.Now);
            // Avoid error when comments array is null
            if (team.Members == null)
            {                
                update = update.Set(doc => doc.Members, members);
            }
            else
            {
                update = update.PushEach(doc => doc.Members, members);
            }

            var result = await _repository.UpdateOneAsync(_collection, filter, update);

            return result.ModifiedCount > 0 ? true : false;
        }

        public async Task<bool> RemoveMemberAsync(string id, IEnumerable<Member> members)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Team>.Update
                        .Set(doc => doc.LastModified, DateTime.Now)
                        .PullAll(doc => doc.Members, members);
                        //.PullFilter(doc => doc.Players, player => player.Id == playerId);
            var options = new FindOneAndUpdateOptions<Team>
            {
               ReturnDocument = ReturnDocument.After
            };
            var result = await _repository.FindOneAndUpdateAsync(_collection, filter, update, options);            

            return result == null ? false : true;
        }

        public async Task<bool> BlockMemberAsync(string id, IEnumerable<Member> members)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Team>.Update
                        .Set(doc => doc.LastModified, DateTime.Now)
                        .PullAll(doc => doc.Members, members)
                        .PullAll(doc => doc.RequestPending, members)
                        .PushEach(doc => doc.BlockedMembers, members);
            //.PullFilter(doc => doc.Players, player => player.Id == playerId);
            var options = new FindOneAndUpdateOptions<Team>
            {
                ReturnDocument = ReturnDocument.After
            };
            var result = await _repository.FindOneAndUpdateAsync(_collection, filter, update, options);

            return result == null ? false : true;
        }

        public async Task<bool> RequestMemberAsync(string id, IEnumerable<Member> members)
        {

            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Team>.Update
                        .Set(doc => doc.LastModified, DateTime.Now)
                        .PushEach(doc => doc.RequestPending, members);            

            var result = await _repository.UpdateOneAsync(_collection, filter, update);

            return result.ModifiedCount > 0 ? true : false;
        }

        public async Task<Team> FindTeamByUser(string userId)
        {
            return await FindOneAsync("Owner.Id", userId);          
        }
    }
}