using MongoDB.Driver;
using System.Configuration;

namespace KickSter
{
    public static class AppConfig
    {
        public static readonly string CONNECTION_STRING          = ConfigurationManager.ConnectionStrings["MongoConnectionString"].ConnectionString;
        public static readonly string DATABASE                   = ConfigurationManager.AppSettings["MongoDatabase"];
        public static readonly string USER_COLLECTION            = ConfigurationManager.AppSettings["UserCollection"];
        public static readonly string ACCOUNT_COLLECTION         = ConfigurationManager.AppSettings["AccountCollection"];
        public static readonly string TEAM_COLLECTION            = ConfigurationManager.AppSettings["TeamCollection"];
        public static readonly string GROUP_COLLECTION           = ConfigurationManager.AppSettings["GroupCollection"];
        public static readonly string ARENA_COLLECTION           = ConfigurationManager.AppSettings["ArenaCollection"];
        public static readonly string ZONE_COLLECTION            = ConfigurationManager.AppSettings["ZoneCollection"];
        public static readonly string POST_COLLECTION            = ConfigurationManager.AppSettings["PostCollection"];
        public static readonly string PREFERENCE_DAY_COLLECTION  = ConfigurationManager.AppSettings["PreferenceDayCollection"];
        public static readonly string USER_MESSAGE_COLLECTION    = ConfigurationManager.AppSettings["UserMessageCollection"];
        public static readonly string SYSTEM_MESSAGE_COLLECTION  = ConfigurationManager.AppSettings["SystemMessageCollection"];
        public static readonly string NEWS_COLLECTION            = ConfigurationManager.AppSettings["NewsCollection"];

        public static readonly string CLIENT_COLLECTION          = ConfigurationManager.AppSettings["ClientCollection"];
        public static readonly string FEEDBACK_COLLECTION        = ConfigurationManager.AppSettings["FeedbackCollection"];
        //public static readonly MongoClientSettings MongoSetting = new MongoClientSettings
        //{
        //    WriteConcern = WriteConcern.Acknowledged
        //};

        public static readonly int DEFAULT_PAGE_SIZE             = int.Parse(ConfigurationManager.AppSettings["DefaultPageSize"]);

        public static readonly string HASH_ALGORITHM             = ConfigurationManager.AppSettings["HashAlgorithm"];
        public static readonly int TOKEN_EXPIRED_IN_MINUTES      = int.Parse(ConfigurationManager.AppSettings["TokenExpiredInMinutes"]);

        // Auth0
        public static readonly string AUTH0_DOMAIN               = ConfigurationManager.AppSettings["Auth0Domain"];
        public static readonly string AUTH0_CLIENT_ID            = ConfigurationManager.AppSettings["Auth0ClientID"];
        public static readonly string AUTH0_CLIENT_SECRET        = ConfigurationManager.AppSettings["Auth0ClientSecret"];
        public static readonly string AUTH0_USER_API             = ConfigurationManager.AppSettings["Auth0UserManagementApi"];
    }
}