using KickSter.Models.Enums;
using System.Collections.Generic;

namespace KickSter.Api.Models
{
    public class ContactInfo
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public List<SocialContact> Socials { get; set; }

    }

    public class SocialContact
    {
        public SocialMedia Name { get; set; }
        public string ContactId { get; set; }
    }
}