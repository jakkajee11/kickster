using MongoDB.Bson.Serialization.Attributes;
using System;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class SystemMessage
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
    }
}