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
        public const string SP_AddNewUser = "dbo.uspAddNewUser";

        public const string SP_DeletePageCategoryMap = "dbo.uspDeletePageCategoryMap";
        public const string SP_DeleteUserPermission = "dbo.uspDeleteUserPermission";

        public const string SP_SearchUserByTextCriteria = "dbo.uspSearchUserByTextCriteria";

        public const string SP_UpdateTransactionFees = "dbo.uspUpdateTransactionFees";
        public const string SP_UpdateUser = "dbo.uspUpdateUser";

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

            public static readonly StatusOption[] StatusOptions = new StatusOption[]
            {
                new StatusOption("IsActive", "AC"),
                new StatusOption("IsActiveDirectoryUser", "AD"),
                new StatusOption("ChangePassword", "CH")
            };

            public static List<string> GetStatusTextsFromCsv(string csv)
            {
                var values = csv.Split(',');
                return values.Select(value => StatusOptions.FirstOrDefault(option => option.Value == value)?.Text)
                        .Where(text => text != null)
                        .Select(text => text!) // Using null-forgiving operator to indicate that nulls are already filtered out
                        .ToList();
            }
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
        public enum SearchByTextStates
        {
            UserId = 1,
            IdNumber = 2,
            FullName = 3,
            Email = 4,
            MobileNo = 5,
            ProfileName = 6
        }
    }
}
