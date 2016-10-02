using MongoDB.Driver;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services
{

    public class GroupService : ServiceBase<Group>, IGroupService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Group> _builder;

        public GroupService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Group>.Filter;
            _collection = AppConfig.GROUP_COLLECTION;
        }

        public async Task<bool> AddTeams(string id, IEnumerable<Team> teams)
        {
            var group = await FindOneAsync(id);

            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Group>.Update
                        .Set(doc => doc.LastModified, DateTime.Now);
            // Avoid error when comments array is null
            if (group.Teams == null)
            {
                update = update.Set(doc => doc.Teams, teams);
            }
            else
            {
                update = update.PushEach(doc => doc.Teams, teams);
            }

            var result = await _repository.UpdateOneAsync(_collection, filter, update);

            return result.ModifiedCount > 0 ? true : false;
        }

        public async Task<bool> RemoveTeam(string id, string teamId)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Group>.Update
                        .Set(doc => doc.LastModified, DateTime.Now)
                        .PullFilter(doc => doc.Teams, team => team.Id == teamId);
            var options = new FindOneAndUpdateOptions<Group>
            {
                ReturnDocument = ReturnDocument.After
            };
            var result = await _repository.FindOneAndUpdateAsync(_collection, filter, update, options);

            return result == null ? false : true;
        }
    }
}