using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using GLib.MongoDB;
using System.Threading.Tasks;
using KickSter.Api.Repository.Interfaces;
using System;
using MongoDB.Bson;

namespace KickSter.Api.Repository
{    
    public class MongoDBRepository : IRepository
    {
        private readonly IMongoConnector _mongo;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDBRepository(IMongoConnector mongo)
        {
            _mongo = mongo;
            _client = mongo.Client();
            _database = _client.GetDatabase(AppConfig.DATABASE);
        }

        public void Insert<T>(string collection, T document)
        {
            _database.GetCollection<T>(collection).InsertOne(document);
        }

        public async Task InsertAsync<T>(string collection, T document)
        {            
            await _database.GetCollection<T>(collection).InsertOneAsync(document);            
        }

        public void InsertMany<T>(string collection, IEnumerable<T> documents)
        {
            _database.GetCollection<T>(collection).InsertMany(documents);
        }

        public async Task InsertManyAsync<T>(string collection, IEnumerable<T> documents)
        {
            await _database.GetCollection<T>(collection).InsertManyAsync(documents);
        }

        public UpdateResult UpdateOne<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
        {
            return _database.GetCollection<T>(collection).UpdateOne(filter, update, options);
        }

        public async Task<UpdateResult> UpdateOneAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
        {            
            return await _database.GetCollection<T>(collection).UpdateOneAsync(filter, update, options);
        }        

        public UpdateResult UpdateMany<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
        {
            return _database.GetCollection<T>(collection).UpdateMany(filter, update, options);
        }

        public async Task<UpdateResult> UpdateManyAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null)
        {
            return await _database.GetCollection<T>(collection).UpdateManyAsync(filter, update, null);
        }                        

        public ReplaceOneResult ReplaceOne<T>(string collection, FilterDefinition<T> filter, T document, UpdateOptions options = null)
        {
            return _database.GetCollection<T>(collection).ReplaceOne(filter, document, options);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync<T>(string collection, FilterDefinition<T> filter, T document, UpdateOptions options = null)
        {
            return _database.GetCollection<T>(collection).ReplaceOneAsync(filter, document, options);
        }

        public List<T> Find<T>(string collection, FilterDefinition<T> filter, FindOptions options = null)
        {
            var document = new List<T>();
            if (_database.GetCollection<T>(collection).Find(filter).Count() > 0)
                document = _database.GetCollection<T>(collection).Find(filter, options).ToList();

            return document;
        }

        public async Task<T> FindOneByIdAsync<T>(string collection, string id)
        {
            var filter = new FilterDefinitionBuilder<T>().Eq("_id", ObjectId.Parse(id));
            var result = await _database.GetCollection<T>(collection).FindAsync(filter);
            return result.FirstOrDefault();
        }

        public async Task<IAsyncCursor<T>> FindAsync<T>(string collection, FilterDefinition<T> filter, FindOptions<T> options = null)
        {            
            return await _database.GetCollection<T>(collection).FindAsync(filter, options);
        }

        public async Task<T> FindOneAndUpdateAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> options = null)
        {
            return await _database.GetCollection<T>(collection).FindOneAndUpdateAsync(filter, update, options);            
        }

        public async Task<T> FindOneAsync<T>(string collection, FilterDefinition<T> filter, FindOptions<T> options = null)
        {            
            var result = await _database.GetCollection<T>(collection).FindAsync(filter, options);

            return await result.FirstOrDefaultAsync();
        }
    }
}