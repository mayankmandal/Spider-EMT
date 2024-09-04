namespace Spider_EMT.Utility
{
    public static class Constants
    {
        public const string MagicString = "";

        public const string JwtCookieName = "_next-session-value";
        public const string JwtRefreshTokenName = "_next-session-token";
        public const string JwtAMRTokenName = "_next-amr-value";

        public const string ClaimAMRKey = "http://schemas.microsoft.com/claims/authnmethodsreferences";
        public const string ClaimAMRValue = "mfa";

        public const string Page_LoginTwoFactorWithAuthenticator = "/Account/LoginTwoFactorWithAuthenticator";
        public const string Page_AuthenticatorWithMFASetup = "/Account/AuthenticatorWithMFASetup";
        public const string Page_UserVerificationSetup = "/Account/UserVerificationSetup";

        public const string BaseUserRoleName = "Base User Access"; // Needs Attention for Change if required for string value

        public const string CategoryType_UncategorizedPages = "Uncategorized Pages";

        public const string SP_AddNewCategory = "dbo.uspAddNewCategory";
        public const string SP_AddNewProfile = "dbo.uspAddNewProfile";
        public const string SP_AddNewUserProfile = "dbo.uspAddNewUserProfile";
        public const string SP_AddPageCategoryMap = "dbo.uspAddPageCategoryMap";
        public const string SP_AddTransactionFees = "dbo.uspAddTransactionFees";
        public const string SP_AddUserPermission = "dbo.uspAddUserPermission";
        public const string SP_AddNewUser = "dbo.uspAddNewUser";

        public const string SP_BaseUserRole = "dbo.uspBaseUserRole";

        public const string SP_CheckUniqueness = "dbo.uspCheckUniqueness";

        public const string SP_DeleteEntityRecord = "dbo.uspDeleteEntityRecord";
        public const string SP_DeletePageCategoryMap = "dbo.uspDeletePageCategoryMap";
        public const string SP_DeleteUserPermission = "dbo.uspDeleteUserPermission";

        public const string SP_InsertNewErrorLog = "dbo.uspInsertNewErrorLog";
        public const string SP_ProvideRoleAccessToUser = "dbo.uspProvideRoleAccessToUser";

        public const string SP_SearchUserByTextCriteria = "dbo.uspSearchUserByTextCriteria";

        public const string SP_UpdateTransactionFees = "dbo.uspUpdateTransactionFees";
        public const string SP_UpdateUser = "dbo.uspUpdateUser";
        public const string SP_UpdateUserSettings = "dbo.uspUpdateUserSettings";
        public const string SP_UpdateUserVerificationInitialSetup = "dbo.uspUpdateUserVerificationInitialSetup";

        public static class PageCategoryMapStates
        {
            public static string PageIdOnly = "PI";
            public static string PageCategoryIdOnly = "PC";
            public static string BothPageIdAndPageCategoryId = "BP";
        };
        public static class UserPermissionStates
        {
            public static int PageIdOnly = 1;
            public static int PageCategoryIdOnly = 2;
            public static int BothPageIdAndPageCategoryId = 3;
        };
        public enum SearchByTextStates
        {
            UserId = 1,
            IdNumber = 2,
            FullName = 3,
            Email = 4,
            MobileNo = 5,
        };
        public enum TableNameCheckUniqueness
        {
            User = 1,
            Profile = 2,
            PageCatagory = 3
        };
        public static class TableNameClassForUniqueness
        {
            public static string[] User = { "idnumber", "email", "mobileno", "username", "userimgpath" };
            public static string[] Profile = { "name" };
            public static string[] PageCatagory = { "catagoryname" };
        };

        public static class BaseUserScreenAccess // Needs Attention for Change if required for string value
        {
            public const string AccessDenied = "/Account/AccessDenied";
            public const string AuthenticatorWithMFASetup = "/Account/AuthenticatorWithMFASetup";
            public const string ConfirmEmail = "/Account/ConfirmEmail";
            public const string Login = "/Account/Login";
            public const string LoginTwoFactorWithAuthenticator = "/Account/LoginTwoFactorWithAuthenticator";
            public const string Logout = "/Account/Logout";
            public const string Register = "/Account/Register";
            public const string UserRoleAssignment = "/Account/UserRoleAssignment";
            public const string UserVerificationSetup = "/Account/UserVerificationSetup";
            public const string Dashboard = "/Dashboard";
            public const string ReadUserProfile = "/ReadUserProfile";
            public const string EditSettings = "/EditSettings";
            public const string Error = "/Error";
        };
    }
}
