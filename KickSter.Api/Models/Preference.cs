using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class Preferences
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string Id { get; set; }
        public List<ZoneBase> Zones { get; set; }
        public List<Arena> Arenas { get; set; }
        //public List<PreferenceDay> Days { get; set; }
        public List<DayTimes> DayTimes { get; set; }
    }

    public class PreferenceDay
    {
        public string Id { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public int Seq { get; set; }
        //public string Day { get; set; }
        //public bool Morning { get; set; }
        //public bool Afternoon { get; set; }
        //public bool Night { get; set; }
        //public bool All { get; set; }
    }
}