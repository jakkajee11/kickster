using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using KickSter.Api.Repository.Interfaces;
using MongoDB.Driver;
using GLib.Common;

namespace KickSter.Api.Services
{
    public class MessageService : ServiceBase<UserMessage>, IMessageService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<UserMessage> _builder;
        private readonly FindOneAndUpdateOptions<UserMessage> _findOneUpdateOptions;

        public MessageService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<UserMessage>.Filter;
            _collection = AppConfig.USER_MESSAGE_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<UserMessage>(true, true);
        }

        public async Task<UserMessage> AddMessageAsync(string id, IEnumerable<Message> messages)
        {
            var filter = Builders<UserMessage>.Filter.Eq(um => um.Id, id);
            var update = Builders<UserMessage>.Update
                .PushEach(um => um.Messages, messages);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
            
        }

        public async Task<IEnumerable<UserMessage>> FindAsync(string id, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<UserMessage>(paging);
            var filter = Builders<UserMessage>
                            .Filter
                            .Eq(msg => msg.Id, id);
            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }
    }
}