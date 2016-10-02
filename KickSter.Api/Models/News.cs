using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class News : EntityBase
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Category { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
    }
}