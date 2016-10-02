using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.Models
{
    public class Message
    {
        public string Text { get; set; }
        public User From { get; set; }
        public DateTime DateSent { get; set; }
    }
}