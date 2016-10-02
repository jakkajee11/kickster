using GLib.Common;
using KickSter.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KickSter.Api.Services.Interfaces
{
    public interface IPostService : IServiceBase<Post>
    {
        

        Task<Post> AddTagAsync(string id, Tag tag);
        Task<Post> AddCommentAsync(string id, Comment comment);
        Task<Post> RemoveCommentAsync(string id, Comment comment);
        Task<Post> AddFollowerAsync(string id, User follower);
        Task<Post> UpdatePlaceAsync(string id, Place place);
        Task<Post> UpdateContactInfoAsync(string id, ContactInfo contactInfo);

        Task<Post> UpdatePictureAsync(string id, IEnumerable<Picture> pictures);        
        Task<Post> UpdateZoneAsync(string id, IEnumerable<ZoneBase> zones);
        Task<Post> UpdateArenaAsync(string id, IEnumerable<Arena> arenas);
        Task<Post> UpdateTagAsync(string id, IEnumerable<Tag> tags);
        Task<Post> UpdatePreferenceDayAsync(string id, IEnumerable<DayTimes> days);

        //Task<IEnumerable<Post>> FindAsync(string field, string value, Paging paging = null);
        Task<IEnumerable<Post>> FindPreferenceDaysAsync(IEnumerable<DayTimes> days, Paging paging = null);
        Task<IEnumerable<Post>> FindArenasAsync(IEnumerable<Arena> arenas, Paging paging = null);
        Task<IEnumerable<Post>> FindZonesAsync(IEnumerable<ZoneBase> zones, Paging paging = null);
        Task<IEnumerable<Post>> FindTagsAsync(IEnumerable<Models.Tag> tags, Paging paging = null);

        Task<List<Post>> GetUserPosts(string id, Paging paging = null);
    }
}