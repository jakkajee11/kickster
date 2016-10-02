using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using KickSter.Api.Repository.Interfaces;
using MongoDB.Driver;
using GLib.Common;

namespace KickSter.Api.Services
{
    public class SystemMessageService : ServiceBase<SystemMessage>, ISystemMessageService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<SystemMessage> _builder;
        private readonly FindOneAndUpdateOptions<SystemMessage> _findOneUpdateOptions;

        public SystemMessageService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<SystemMessage>.Filter;
            _collection = AppConfig.USER_MESSAGE_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<SystemMessage>(true, true);
        }                

        public async Task<IEnumerable<SystemMessage>> FindAsync(string id, Paging paging)
        {
            var options = ServiceHelpers.BuildFindOptions<SystemMessage>(paging);
            var filter = Builders<SystemMessage>
                            .Filter
                            .Eq(msg => msg.Id, id);
            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }
    }
}