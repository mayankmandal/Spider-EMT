namespace Spider_EMT.Utility
{
    public static class Constants
    {
        public const string MagicString = "";
        public const string SP_AddTransactionFees = "dbo.uspAddTransactionFees";
        public const string SP_UpdateTransactionFees = "dbo.uspUpdateTransactionFees";
        public const string SP_AddUserProfile = "dbo.uspAddUserProfile";
        public const string SP_AddUserPermission = "dbo.uspAddUserPermission";
        public const string SP_UpdateUserProfile = "dbo.uspUpdateUserProfile";
        public const string SP_DeleteUserPermission = "dbo.uspDeleteUserPermission";
        public const string SP_AddNewUserProfile = "dbo.uspAddNewUserProfile";
        public const string SP_DeleteUserProfile = "dbo.uspDeleteUserProfile";
        public const string SP_AddNewCategory = "dbo.uspAddNewCategory";
        public const string SP_DeletePageCategoryMap = "dbo.uspDeletePageCategoryMap";
        public const string SP_AddPageCategoryMap = "dbo.uspAddPageCategoryMap";
        public static class UserStatus
        {
            public static string IsActive = "AC";
            public static string IsActiveDirectoryUser = "AD";
            public static string ChangePassword = "CH";
        };
        public static class DeletePageCategoryMap
        {
            public static string PageIdOnly = "PI";
            public static string PageCategoryIdOnly = "PC";
            public static string BothPageIdAndPageCategoryId = "BP";
        };
    }
}
