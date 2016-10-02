using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.Models
{
    public class DayTimes
    {
        public string Day { get; set; }
        public bool Morning { get; set; }
        public bool Afternoon { get; set; }
        public bool Night { get; set; }
        //public bool All { get; set; }
    }
}