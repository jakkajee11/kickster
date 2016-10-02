using System.Security.Principal;
using System.Web;

namespace KickSter.Api.Helpers
{
    public static class UserHelper
    {
        public static IIdentity CurrentUser
        {
            get
            {
                return HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity : null;
            }
        }
    }
}