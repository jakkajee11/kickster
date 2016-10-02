using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Comment //: PostBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Message { get; set; }
        public User CommentedBy { get; set; }
        public DateTime DateCreated { get; set; }
        //public List<Comment> Replies { get; set; }
    }
}