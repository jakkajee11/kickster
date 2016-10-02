using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System;
using System.Threading.Tasks;
using KickSter.Api.Repository.Interfaces;
using MongoDB.Driver;

namespace KickSter.Api.Services
{
    public class ClientService : ServiceBase<Client>, IClientService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Client> _builder;
        private readonly FindOneAndUpdateOptions<Client> _findOneUpdateOptions;

        public ClientService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Client>.Filter;
            _collection = AppConfig.CLIENT_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<Client>(true);
        }

        public async Task<Client> Create(Client client)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, client.Id);
            var update = Builders<Client>.Update
                        .Set(doc => doc, client);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<bool> IsExist(string clientId, string domain)
        {
            var result = await _repository.FindOneByIdAsync<Client>(_collection, clientId);
            if (result != null)
            {
                return result.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}