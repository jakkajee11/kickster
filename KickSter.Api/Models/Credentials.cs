using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using KickSter.Models;

namespace KickSter.Api.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Account : EntityBase//, IUser<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }       
        public string Token { get; set; }
        public string Salt { get; set; }
        public DateTime? DateTokenExpired { get; set; }
        public Player Player { get; set; }
    }    
}