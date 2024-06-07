using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                if(file.Length > _maxFileSize)
                {
                    return new ValidationResult(ErrorMessage ?? $"File size cannot exceed {_maxFileSize / 1024} KB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
