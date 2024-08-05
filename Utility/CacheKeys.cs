using Spider_EMT.Repository.Domain;

namespace Spider_EMT.Utility
{
    public static class CacheKeys
    {
        public static string CurrentUserKey = "CurrentUser";
        public static string CurrentUserProfileKey = "CurrentUserProfile";
        public static string CurrentUserPagesKey = "CurrentUserPages";
        public static string CurrentUserCategoriesKey = "CurrentUserCategories";
    }
    public static class JWTCookieHelper
    {
        public static string GetJWTCookie(HttpContext httpContext)
        {
            return httpContext.Request.Cookies[Constants.JwtCookieName];
        }
    }
}
