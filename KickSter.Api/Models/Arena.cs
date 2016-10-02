using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Arena
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        //public string Detail { get; set; }
        public ZoneBase Zone { get; set; }        
        public Location Location { get; set; }
        public List<Picture> Photos { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }

    public class ArenaInfo //: Arena
    {
        public string Detail { get; set; }
        public Zone Zone { get; set; }
        public Location Location { get; set; }
        public List<Picture> Photos { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }
}