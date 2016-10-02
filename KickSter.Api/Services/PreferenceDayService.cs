using MongoDB.Driver;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GLib.Core.Hash;

namespace KickSter.Api.Services
{
    public class PreferenceDayService : ServiceBase<PreferenceDay>, IPreferenceDayService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<PreferenceDay> _builder;

        public PreferenceDayService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<PreferenceDay>.Filter;
            _collection = AppConfig.PREFERENCE_DAY_COLLECTION;
        }

        public async override Task<PreferenceDay> CreateAsync(PreferenceDay day)
        {
            var existPref = await FindOneAsync("Id", day.Id);

            if (existPref == null)
                await _repository.InsertAsync(_collection, day);

            return day;
        }

        

        public override async Task ImportAsync(IEnumerable<PreferenceDay> days)
        {
            // import only document that doesn't already exists            
            var allDays = await ListAsync();            
            var newDays = days.ToList();
            
            newDays = newDays.Where(pf => !allDays.Exists(ad => ad.Id.Equals(pf.Id))).ToList();

            if (newDays.Count() > 0)
                await _repository.InsertManyAsync(_collection, newDays);
        }
    }
}