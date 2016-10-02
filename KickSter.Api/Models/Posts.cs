using KickSter.Models;
using KickSter.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class PostBase : EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Message { get; set; }
        //public PostType Type { get; set; }
        public string Type { get; set; }
        public bool Pinned { get; set; }
        public PostStatus Status { get; set; }
        public User PostedBy { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Post : PostBase
    {
        public Place Place { get; set; }
        public ContactInfo ContactInfo { get; set; }
        //public List<PreferenceDay> Days { get; set; }
        
        public List<Tag> Tags { get; set; }
        public List<Picture> Pictures { get; set; }
        public List<User> Followers { get; set; }
        public List<DayTimes> DayTimes { get; set; }
        public List<ZoneBase> Zones { get; set; }
        public List<Arena> Arenas { get; set; }        
        public List<Comment> Comments { get; set; }   
        public TeamOption TeamOption { get; set; }     
    }

    public class MatchedPost : Post
    {
        public int Score { get; set; }
        public bool Day { get; set; }
        public bool Arena { get; set; }
        public bool Zone { get; set; }
    }

    public class TeamOption
    {
        public int MaxMember { get; set; }
        public int MaxTeam { get; set; }
    }
}