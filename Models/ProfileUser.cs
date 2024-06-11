using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Models
{
    public class ProfileUser
    {
        public int UserId { get; set; }
        public string IdNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public ProfileSite ProfileSiteData { get; set; }
        public string Username { get; set; }
        public string Userimgpath { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public bool ChangePassword { get; set; }
        public DateTime? LastLoginActivity { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
