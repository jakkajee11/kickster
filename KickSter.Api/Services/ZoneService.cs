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
    public class ZoneService : ServiceBase<Zone>, IZoneService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Zone> _builder;

        public ZoneService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Zone>.Filter;
            _collection = AppConfig.ZONE_COLLECTION;
        }

        public async override Task<Zone> CreateAsync(Zone zone)
        {
            await _repository.InsertAsync(_collection, zone);

            return zone;
        }

        //public async Task<bool> AddLanguages(string id, IEnumerable<ZoneDetail> newLanguages)
        //{
        //    var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
        //    var update = Builders<Zone>.Update
        //                .Set(doc => doc.LastModified, DateTime.Now)
        //                .PushEach("Languages", newLanguages);            
        //    var result = await _repository.FindOneAndUpdateAsync(_collection, filter, update);            

        //    return result != null ? true : false;
        //}

        public override async Task ImportAsync(IEnumerable<Zone> zones)
        {
            // import only document that doesn't already exists            
            var allZones = await ListAsync();

            //var newZones = zones;//.Where(zn => zn.Detail != null);
            //newZones = newZones.Where(zn => !allZones.Exists(az => az.Detail.JsonHash(false).Equals(zn.Detail.JsonHash(false))));
            var newZones = zones.Where(zn => !allZones.Exists(az => HashHelpers.RSHash(az.Country.ToLower(), az.City.ToLower(), az.Province.ToLower()).Equals(HashHelpers.RSHash(zn.Country.ToLower(), zn.City.ToLower(), zn.Province.ToLower()))));

            if (newZones.Count() > 0)
                await _repository.InsertManyAsync(_collection, newZones);
        }
    }
}