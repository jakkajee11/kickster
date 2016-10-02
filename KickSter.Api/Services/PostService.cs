using MongoDB.Driver;
using KickSter.Api.Services.Interfaces;
using KickSter.Api.Repository.Interfaces;
using KickSter.Api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using GLib.Common;
using System.Linq;
using MongoDB.Bson;

namespace KickSter.Api.Services
{

    public class PostService : ServiceBase<Post>, IPostService
    {
        readonly IRepository _repository;
        readonly FilterDefinitionBuilder<Post> _builder;
        private readonly FindOneAndUpdateOptions<Post> _findOneUpdateOptions;

        public PostService(IRepository repository) : base(repository)
        {
            _repository = repository;
            _builder = Builders<Post>.Filter;
            _collection = AppConfig.POST_COLLECTION;
            _findOneUpdateOptions = ServiceHelpers.BuildFindOneAndUpdateOptions<Post>(true);
        }                

        public async Task<Post> AddCommentAsync(string id, Comment comment)
        {
            // Set comment date
            comment.Id = ObjectId.GenerateNewId().ToString();
            comment.DateCreated = DateTime.Now;

            var comments = new List<Comment>();
            comments.Add(comment);

            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update                        
                        .PushEach(doc => doc.Comments, comments);
                        
            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);            
        }

        public async Task<Post> RemoveCommentAsync(string id, Comment comment)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);

            var update = Builders<Post>
                            .Update
                            .PullFilter(p => p.Comments, p => p.Id == comment.Id);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> AddFollowerAsync(string id, User follower)
        {

            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.LastModified, DateTime.Now);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdatePlaceAsync(string id, Place place)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Place, place);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdatePictureAsync(string id, IEnumerable<Picture> pictures)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Pictures, pictures);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> AddTagAsync(string id, Models.Tag tag)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.LastModified, DateTime.Now);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdateZoneAsync(string id, IEnumerable<ZoneBase> zones)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Zones, zones);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdateArenaAsync(string id, IEnumerable<Arena> arenas)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Arenas, arenas);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }        

        public async Task<Post> UpdateTagAsync(string id, IEnumerable<Models.Tag> tags)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Tags, tags);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdateContactInfoAsync(string id, ContactInfo contactInfo)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.ContactInfo, contactInfo);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public async Task<Post> UpdatePreferenceDayAsync(string id, IEnumerable<DayTimes> days)
        {
            var filter = ServiceHelpers.BuildKeyFilter(_builder, id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.DayTimes, days);

            return await _repository.FindOneAndUpdateAsync(_collection, filter, update, _findOneUpdateOptions);
        }

        public override async Task<IEnumerable<Post>> FindAnyInAsync<TField>(string field, IEnumerable<TField> values, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);
            var filter = Builders<Post>
                            .Filter
                            .AnyIn(field, values);
                            
            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Post>> FindPreferenceDaysAsync(IEnumerable<DayTimes> days, Paging paging = null)
        {            
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);
            var filterDay =
                    Builders<Post>
                        .Filter
                        .In("DayTimes.Day", days.Select(d => d.Day));
            var filterTimes = new List<FilterDefinition<Post>>();

            foreach(var t in days.Where(dt=> dt.Morning || dt.Afternoon || dt.Night))
            {
                if (t.Morning)
                {
                    filterTimes.Add
                        (
                            Builders<Post>
                                .Filter
                                .Eq("DayTimes.Morning", true)
                        );
                } else if (t.Afternoon)
                {
                    filterTimes.Add
                        (
                            Builders<Post>
                                .Filter
                                .Eq("DayTimes.Afternoon", true)
                        );
                } else if (t.Night)
                {
                    filterTimes.Add
                        (
                            Builders<Post>
                                .Filter
                                .Eq("DayTimes.Night", true)
                        );
                }
                
            }                       

            var filters = Builders<Post>
                            .Filter
                            .And
                            (
                                filterDay,                                
                                Builders<Post>.Filter.Or(filterTimes)
                            );               

            var result = await _repository.FindAsync(_collection, filters, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Post>> FindArenasAsync(IEnumerable<Arena> arenas, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);

            var filter = Builders<Post>
                            .Filter
                            .In("Arenas.Id", arenas.Select(an => ObjectId.Parse(an.Id)));

            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Post>> FindZonesAsync(IEnumerable<ZoneBase> zones, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);
            var filter = Builders<Post>
                            .Filter
                            .In("Zones.Id", zones.Select(zn => zn.Id));
            //.In("Zones.Id", zones.Select(zn => ObjectId.Parse(zn.Id)));

            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<Post>> FindTagsAsync(IEnumerable<Models.Tag> tags, Paging paging = null)
        {
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);
            var filter = Builders<Post>
                            .Filter
                            .In("Tags.Name", tags.Select(tg => tg.Name));
            //.In("Tags.Name", tags.Select(tg => ObjectId.Parse(tg.Id)));

            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }

        public override async Task ImportAsync(IEnumerable<Post> posts)
        {
            foreach(var post in posts)
            {
                if (post.DateCreated == DateTime.MinValue) post.DateCreated = DateTime.Now;
                if (post.Comments == null) post.Comments = new List<Comment>();
            }
            await _repository.InsertManyAsync(_collection, posts);
        }

        public override async Task<Post> CreateAsync(Post post)
        {
            if (post.Comments == null) post.Comments = new List<Comment>();
            if (post.Arenas == null) post.Arenas = new List<Arena>();
            if (post.Zones == null) post.Zones = new List<ZoneBase>();
            if (post.Tags == null) post.Tags = new List<Models.Tag>();
            if (post.Pictures == null) post.Pictures = new List<Picture>();
            if (post.DayTimes == null) post.DayTimes = new List<DayTimes>();
            if (post.Followers == null) post.Followers = new List<User>();
            if (post.DateCreated == DateTime.MinValue) post.DateCreated = DateTime.Now;

            await _repository.InsertAsync(_collection, post);

            return post;
        }

        public override async Task<Post> UpdateAsync(Post post)
        {
            if (post.Comments == null) post.Comments = new List<Comment>();
            if (post.Arenas == null) post.Arenas = new List<Arena>();
            if (post.Zones == null) post.Zones = new List<ZoneBase>();
            if (post.Tags == null) post.Tags = new List<Models.Tag>();
            if (post.Pictures == null) post.Pictures = new List<Picture>();
            if (post.DayTimes == null) post.DayTimes = new List<DayTimes>();
            if (post.Followers == null) post.Followers = new List<User>();
            //if (post.DateCreated == DateTime.MinValue) post.DateCreated = DateTime.Now;
            var filter = ServiceHelpers.BuildKeyFilter(_builder, post.Id);
            var update = Builders<Post>.Update
                        .Set(doc => doc.Type, post.Type)
                        .Set(doc => doc.Message, post.Message)
                        .Set(doc => doc.DayTimes, post.DayTimes)
                        .Set(doc => doc.Arenas, post.Arenas)
                        .Set(doc => doc.Zones, post.Zones)
                        .Set(doc => doc.DayTimes, post.DayTimes)
                        .Set(doc => doc.LastModified, DateTime.Now);

            await _repository.UpdateOneAsync(_collection, filter, update);

            return post;
        }

        public async Task<List<Post>> GetUserPosts(string id, Paging paging = null)
        {
            var filter = _builder.Eq(p => p.PostedBy.Id, id);
            var options = ServiceHelpers.BuildFindOptions<Post>(paging);
            var result = await _repository.FindAsync(_collection, filter, options);

            return await result.ToListAsync();
        }
    }
}