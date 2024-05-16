namespace Spider_EMT.Utility
{
    public static class Constants
    {
        public const string MagicString = "";

        public const string SP_AddNewCategory = "dbo.uspAddNewCategory";
        public const string SP_AddNewUserProfile = "dbo.uspAddNewUserProfile";
        public const string SP_AddNewProfile = "dbo.uspAddNewProfile";
        public const string SP_AddPageCategoryMap = "dbo.uspAddPageCategoryMap";
        public const string SP_AddTransactionFees = "dbo.uspAddTransactionFees";
        public const string SP_AddUserPermission = "dbo.uspAddUserPermission";
        public const string SP_AddUserProfile = "dbo.uspAddUserProfile";

        public const string SP_DeletePageCategoryMap = "dbo.uspDeletePageCategoryMap";
        public const string SP_DeleteUserPermission = "dbo.uspDeleteUserPermission";
        public const string SP_DeleteUserProfile = "dbo.uspDeleteUserProfile";

        public const string SP_UpdateTransactionFees = "dbo.uspUpdateTransactionFees";
        public const string SP_UpdateUserProfile = "dbo.uspUpdateUserProfile";

        public static class UserStatusDescription
        {
            public class StatusOption
            {
                public string Text { get; set; }
                public string Value { get; set; }
                public StatusOption(string text, string value)
                {
                    Text = text;
                    Value = value;
                }
            }
            public static StatusOption IsActive = new StatusOption("IsActive","AC");
            public static StatusOption IsActiveDirectoryUser = new StatusOption("IsActiveDirectoryUser", "AD");
            public static StatusOption ChangePassword = new StatusOption("ChangePassword", "CH");
        }
        
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
    }
}
