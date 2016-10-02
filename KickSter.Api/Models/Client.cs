using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Client
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]        
        public string Id { get; set; }
        [BsonElement("domain")]
        public string Domain { get; set; }
    }
}