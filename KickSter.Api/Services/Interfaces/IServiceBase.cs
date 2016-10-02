using GLib.Common;
using KickSter.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IServiceBase<T>
    {

        Task<List<T>> ListAsync(Paging paging = null);

        Task<T> FindOne(ObjectId id);

        Task<T> FindOneAsync(string id);

        Task<T> FindOneAsync(string field, string value);

        Task<IEnumerable<T>> FindAsync(FindOpt findOpt);

        Task<IEnumerable<T>> FindAsync(string field, string value);

        Task<IEnumerable<T>> FindAsync(string collection, FilterDefinition<T> filter, FindOptions<T> options = null);

        Task<T> CreateAsync(T document);

        Task<T> UpdateAsync(T document);

        Task<T> ReplaceAsync(T document);

        Task ImportAsync(IEnumerable<T> documents);

        Task<T> FindOneAndUpdateAsync(T document);

        Task<IEnumerable<T>> FindObjectIdInAsync(IEnumerable<ObjectId> values, Paging paging = null);

        Task<IEnumerable<T>> FindInAsync<TField>(string field, IEnumerable<TField> values, Paging paging = null);

        Task<IEnumerable<T>> FindAnyInAsync<TField>(string field, IEnumerable<TField> values, Paging paging = null);

    }
}