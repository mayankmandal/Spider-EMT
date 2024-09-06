namespace Spider_EMT.Utility
{
    public static class SessionKeys
    {
        public static string CurrentUserKey = "CurrentUser";
        public static string CurrentUserProfileKey = "CurrentUserProfile";
        public static string CurrentUserPagesKey = "CurrentUserPages";
        public static string CurrentUserCategoriesKey = "CurrentUserCategories";
        public static string CurrentUserClaimsKey = "CurrentUserClaims";
    }
    public static class JWTCookieHelper
    {
        public static string GetJWTCookie(HttpContext httpContext)
        {
            return httpContext.Request.Cookies[Constants.JwtCookieName];
        }
        public static string GetJWTAMRToken(HttpContext httpContext)
        {
            return httpContext.Request.Cookies[Constants.JwtAMRTokenName];
        }
    }
}
