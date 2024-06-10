using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;

namespace Spider_EMT.Models.ViewModels
{
    public class SettingsViewModel
    {
        [DisplayName("User Id")]
        public int SettingId { get; set; }
        [DisplayName("Full Name")]
        public string SettingName { get; set; }
        [DisplayName("Username")]
        public string SettingUsername { get; set; }
        [DisplayName("Email Address")]
        public string SettingEmail { get; set; }
        [DisplayName("User Photo")]
        [MaxFileSize(20 * 1024, ErrorMessage = "Image size cannot exceed 20 KB")]
        public IFormFile SettingPhotoFile { get; set; }
    }
}
