using KickSter.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        public string Id { get; set; }
        public string NickName { get; set; }
        //public string UserName { get; set; }
        //public Picture Picture { get; set; }
        public string Picture { get; set; }

        //public string DisplayName
        //{
        //    get
        //    {
        //        return string.IsNullOrEmpty(this.NickName) ? this.UserName : this.NickName;
        //    }
        //    private set { }
        //}
    }
    [BsonIgnoreExtraElements]
    public class UserProfile : EntityBase
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Role { get; set; }
        //public string UserId { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }
        public DateTime? DateTokenExpired { get; set; }
        //public string UserType { get; set; }
        public string UserType
        {
            get
            {
                return Id == null ? null : Id.Split(new char[] { '|' })[0];
            }
        }
        //public Picture Picture { get; set; }
        public string Picture { get; set; }
        //public Gender Gender { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Preferences Preferences { get; set; }
        public Social Social { get; set; }
        public string LastIp { get; set; }
        public DateTime? LastLogin { get; set; }
        public int TotalPost { get; set; }
        public bool? IsPublic { get; set; }

        public bool FirstTimeLogin
        {
            get
            {
                return !LastLogin.HasValue;
            }
        }
    }
    [BsonIgnoreExtraElements]
    public class UserInfo// : EntityBase
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }        
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Picture { get; set; }
        //public Gender Gender { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public Preferences Preferences { get; set; }
        public Social Social { get; set; }
        public int TotalPost { get; set; }
    }

    public class Social
    {
        public string Facebook { get; set; }
        public string Line { get; set; }
    }

    public class Auth0UserMetaData
    {
        public string user_metadata { get; set; }
    }

    public class UserMessage
    {
        [BsonId]
        public string Id { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}