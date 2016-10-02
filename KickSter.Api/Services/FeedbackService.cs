using MongoDB.Driver;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;
using System;
using System.Threading.Tasks;

namespace KickSter.Api.Services
{
    public class FeedbackService : ServiceBase<Feedback>, IFeedbackService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Feedback> _builder;

        public FeedbackService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Feedback>.Filter;
            _collection = AppConfig.FEEDBACK_COLLECTION;
        }

        public override async Task<Feedback> CreateAsync(Feedback feedback)
        {
            feedback.DateCreated = DateTime.Now;
            return await base.CreateAsync(feedback);
        }
    }
}