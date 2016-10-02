using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    public class ZoneBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        //public string Zip { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public float Lat { get; set; }
        [BsonRepresentation(BsonType.Double)]
        public float Lng { get; set; }
    }
    //[BsonIgnoreExtraElements]
    //[Serializable]
    //[BsonSerializer(typeof(BsonDefaults))]    
    public class Zone : EntityBase
    {        
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        //public string Zip { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
    }

    //[Serializable]
    //[BsonSerializer(typeof(BsonDefaults))]
    public class ZoneDetail
    {
        public string Language { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        //public string HashKey { get; set; }
        
    }
}