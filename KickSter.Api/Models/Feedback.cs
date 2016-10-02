using MongoDB.Bson.Serialization.Attributes;
using System;

namespace KickSter.Api.Models
{
    public class Feedback
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Message { get; set; }
        public User FromUser { get; set; }
        public DateTime DateCreated { get; set; }
    }
}