using GLib.Common;
using KickSter.Models;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KickSter.Api.Services
{
    public abstract class ServiceBase<T> : IServiceBase<T>
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<T> _builder;
        protected string _collection;
        public ServiceBase(IRepository repository)
        {
            _repository = repository;
            _builder = Builders<T>.Filter;
        }

        public virtual async Task<List<T>> ListAsync(Paging paging = null)
        {
            var findOptions = ServiceHelpers.BuildFindOptions<T>(paging);
            var result = await _repository.FindAsync(_collection, FilterDefinition<T>.Empty, findOptions);
            return await result.ToListAsync();//result.ToEnumerable();
        }

        public virtual async Task<T> FindOne(ObjectId id)
        {
            var filter = _builder.Eq("Id", id);
            var result = await _repository.FindAsync(_collection, filter);
            return result.FirstOrDefault();
        }

        public virtual async Task<T> FindOneAsync(string id)
        {
            return await FindOne(ObjectId.Parse(id));
        }

        public virtual async Task<IEnumerable<T>> FindAsync(FindOpt findOpt)
        {
            var filter = _builder.Eq(findOpt.Field, findOpt.Value);

            var findOptions = ServiceHelpers.BuildFindOptions<T>(findOpt.Paging);
            var result = await _repository.FindAsync(_collection, filter, findOptions);

            return result.ToEnumerable();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(string field, string value)
        {
            var filter = _builder.Eq(field, value);
            var result = await _repository.FindAsync(_collection, filter);

            return result.ToEnumerable();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(string collection, FilterDefinition<T> filter, FindOptions<T> options = null)
        {            
            var result = await _repository.FindAsync(_collection, filter, options);

            return result.ToEnumerable();
        }

        public virtual async Task<T> CreateAsync(T document)
        {
            await _repository.InsertAsync(_collection, document);

            return document;
        }

        public virtual async Task<T> UpdateAsync(T document)
        {
            var field = document.GetType().GetProperty("Id");            
            var bson = document.ToBsonDocument();
            var filter = ServiceHelpers.BuildKeyFilter(_builder, field.GetValue(document).ToString());
            // Set LastModified property to Current datetime
            var lastMod = document.GetType().GetProperty("LastModified");
            if (lastMod != null)
                lastMod.SetValue(document, DateTime.Now);

            var update = Builders<T>.Update
                        .Set(doc => doc, document);
            

            var result = await _repository.UpdateOneAsync(_collection, filter, update);
            if (result.ModifiedCount == 0) document = default(T);

            return document;
        }

        public virtual async Task<T> ReplaceAsync(T document)
        {
            var field = document.GetType().GetProperty("Id");
            var filter = ServiceHelpers.BuildKeyFilter(_builder, field.GetValue(document).ToString());
            var result = await _repository.ReplaceOneAsync(_collection, filter, document);
            if (result.ModifiedCount == 0) document = default(T);

            return document;
        }

        public virtual async Task ImportAsync(IEnumerable<T> documents)
        {            
            await _repository.InsertManyAsync(_collection, documents);
        }

        public virtual async Task<T> FindOneAndUpdateAsync(T document)
        {
            var field = document.GetType().GetProperty("Id");
            var filter = ServiceHelpers.BuildKeyFilter(_builder, field.GetValue(document).ToString());
            // Set LastModified property to Current datetime
            var lastMod = document.GetType().GetProperty("LastModified");
            if (lastMod != null)
                lastMod.SetValue(document, DateTime.Now);

            var update = Builders<T>.Update
                        .Set(doc => doc, document);
            var options = new FindOneAndUpdateOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            };

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, options);                        
        }

        public async virtual Task<IEnumerable<T>> FindObjectIdInAsync(IEnumerable<ObjectId> values, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<T>(paging);
            var filter = Builders<T>
                            .Filter
                            .In("_id", values);

            var result = await _repository.FindAsync(_collection, filter, options);

            return result.ToEnumerable();
        }

        public async virtual Task<IEnumerable<T>> FindInAsync<TField>(string field, IEnumerable<TField> values, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<T>(paging);
            var filter = Builders<T>
                            .Filter
                            .In(field, values);

            var result = await _repository.FindAsync(_collection, filter, options);

            return result.ToEnumerable();
        }

        public async virtual Task<IEnumerable<T>> FindAnyInAsync<TField>(string field, IEnumerable<TField> values, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<T>(paging);
            var filter = Builders<T>
                            .Filter
                            .AnyIn(field, values);

            var result = await _repository.FindAsync(_collection, filter, options);

            return result.ToEnumerable();
        }

        public async Task<T> FindOneAsync(string field, string value)
        {
            var filter = _builder.Eq(field, value);
            return await _repository.FindOneAsync(_collection, filter);
        }
    }
}