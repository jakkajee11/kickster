using GLib.Common;
using KickSter.Api.Models;
using KickSter.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace KickSter.Models
{
    public class Player
    {
        [BsonIgnore]        
        public string Id { get; set; }
        public string Name { get; set; }
        public Picture Avatar { get; set; }
        public AdditionalInfo OtherInfo { get; set; }
        public Preferences Preferences { get; set; }       
    }

    public class AdditionalInfo
    {
        //public Picture Avatar { get; set; }
        public DateTime BirthDay { get; set; }
        public Gender Gender { get; set; }
    }

    

    

    

    

    

    

    

    

    

    

    

    

    public class KeyValues
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class FindOpt
    {
        public string Field { get; set; }
        public object Value { get; set; }
        public Paging Paging { get; set; }
    }
}