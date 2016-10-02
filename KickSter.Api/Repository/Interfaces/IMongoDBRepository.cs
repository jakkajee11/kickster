using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Repository.Interfaces
{
    public interface IRepository
    {
        void Insert<T>(string collection, T document);
        Task InsertAsync<T>(string collection, T document);
        void InsertMany<T>(string collection, IEnumerable<T> documents);
        Task InsertManyAsync<T>(string collection, IEnumerable<T> documents);
        UpdateResult UpdateOne<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null);
        Task<UpdateResult> UpdateOneAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null);        
        Task<UpdateResult> UpdateManyAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null);
        ReplaceOneResult ReplaceOne<T>(string collection, FilterDefinition<T> filter, T document, UpdateOptions options = null);
        Task<ReplaceOneResult> ReplaceOneAsync<T>(string collection, FilterDefinition<T> filter, T document, UpdateOptions options = null);
        List<T> Find<T>(string collection, FilterDefinition<T> filter, FindOptions options = null);
        Task<T> FindOneByIdAsync<T>(string collection, string id);
        Task<T> FindOneAsync<T>(string collection, FilterDefinition<T> filter, FindOptions<T> options = null);
        Task<IAsyncCursor<T>> FindAsync<T>(string collection, FilterDefinition<T> filter, FindOptions<T> options = null);
        Task<T> FindOneAndUpdateAsync<T>(string collection, FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> options = null);
    }
}