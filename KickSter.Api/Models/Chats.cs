using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public User CreatedBy { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<ChatMessage> Messages { get; set; }
    }

    public class ChatMessage
    {
        public string Message { get; set; }
        public User CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}