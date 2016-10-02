using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    public class Team : EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Member> Members { get; set; }
        public List<Member> RequestPending { get; set; }
        public List<Member> BlockedMembers { get; set; }
        public User Owner { get; set; }
    }

    public class Member
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public bool Blocked { get; set; }
        public DateTime? DateAccepted { get; set; }
        public DateTime? DateRequested { get; set; }
    }
}