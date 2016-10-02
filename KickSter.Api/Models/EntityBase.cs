using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Api.Models
{
    public class EntityBase
    {
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime DateChanged
        {
            get
            {
                return (LastModified.HasValue && LastModified.Value > DateCreated) ? LastModified.Value : DateCreated;
            }
        }
    }
}