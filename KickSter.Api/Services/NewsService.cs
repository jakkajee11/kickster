using KickSter.Api.Models;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KickSter.Api.Services
{
    public class NewsService : ServiceBase<News>, INewsService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<News> _builder;
        private readonly FindOneAndUpdateOptions<News> _findOneUpdateOptions;

        public NewsService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<News>.Filter;
            _collection = AppConfig.NEWS_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<News>(true);
        }

        public override async Task<News> CreateAsync(News news)
        {
            if (news.DateCreated == DateTime.MinValue) news.DateCreated = DateTime.Now; 
            await _repository.InsertAsync(_collection, news);
            return news;    
        }
    }
}