using System;
using MongoDB.Driver;
using System.Collections.Generic;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;

namespace KickSter.Api.Services
{

    public class ArenaService : ServiceBase<Arena>, IArenaService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Arena> _builder;

        public ArenaService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Arena>.Filter;
            _collection = AppConfig.ARENA_COLLECTION;
        }

        public List<Arena> Search(Dictionary<string, object> keyValues)
        {
            throw new NotImplementedException();
        }
    }
}