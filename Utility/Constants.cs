namespace Spider_EMT.Utility
{
    public static class Constants
    {
        public const string MagicString = "";

        public const string CategoryType_UncategorizedPages = "Uncategorized Pages";

        public const string SP_AddNewCategory = "dbo.uspAddNewCategory";
        public const string SP_AddNewProfile = "dbo.uspAddNewProfile";
        public const string SP_AddPageCategoryMap = "dbo.uspAddPageCategoryMap";
        public const string SP_AddTransactionFees = "dbo.uspAddTransactionFees";
        public const string SP_AddUserPermission = "dbo.uspAddUserPermission";
        public const string SP_AddNewUser = "dbo.uspAddNewUser";

        public const string SP_CheckUniqueness = "dbo.uspCheckUniqueness";

        public const string SP_DeleteEntityRecord = "dbo.uspDeleteEntityRecord";
        public const string SP_DeletePageCategoryMap = "dbo.uspDeletePageCategoryMap";
        public const string SP_DeleteUserPermission = "dbo.uspDeleteUserPermission";

        public const string SP_SearchUserByTextCriteria = "dbo.uspSearchUserByTextCriteria";

        public const string SP_UpdateTransactionFees = "dbo.uspUpdateTransactionFees";
        public const string SP_UpdateUser = "dbo.uspUpdateUser";
        public const string SP_UpdateUserSettings = "dbo.uspUpdateUserSettings";

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
            ProfileName = 6
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
            public static string[] Profile = { "profilename" };
            public static string[] PageCatagory = { "categoryname" };
        };

    }
}
