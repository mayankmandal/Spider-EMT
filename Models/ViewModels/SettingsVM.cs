using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class SettingsVM
    {
        [DisplayName("User Id")]
        public int SettingId { get; set; }
        [DisplayName("Full Name")]
        [StringLength(200, ErrorMessage = "Full Name must be 200 characters or fewer")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full Name must contain only alphabets and spaces")]
        public string SettingName { get; set; }
        [DisplayName("Username")]
        [StringLength(100, ErrorMessage = "Username must be 100 characters or fewer")]
        [RegularExpression(@"^[a-zA-Z0-9._ ]*$", ErrorMessage = "Username must contain only alphabets, numbers, spaces, period and underscores")]
        public string SettingUsername { get; set; }
        [DisplayName("Email Address")]
        [StringLength(100, ErrorMessage = "Email Address must be 100 characters or fewer")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string SettingEmail { get; set; }
        [DisplayName("User Photo")]
        [MaxFileSize(20 * 1024, ErrorMessage = "Image size cannot exceed 20 KB")]
        public IFormFile SettingPhotoFile { get; set; }
        
        [DisplayName("New Password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(16, ErrorMessage = "Password must be at most 16 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string? Password {  get; set; }
        [DisplayName("ReType New Password")]
        [MinLength(8, ErrorMessage = "ReType Password must be at least 8 characters long")]
        [MaxLength(16, ErrorMessage = "ReType Password must be at most 16 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "ReType Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [Compare("Password", ErrorMessage = "New Password and ReType New Password do not match.")]
        public string? ReTypePassword {  get; set; }
    }
}
