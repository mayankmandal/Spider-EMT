using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Spider_EMT.Models.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Spider_EMT.Models.ViewModels
{
    public class ProfileUserVM
    {
        [DisplayName("User ID")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "ID Number is required")]
        [DisplayName("Iqama Number")]
        [Range(1000000000, 9999999999, ErrorMessage = "ID Number must be a 10-digit integer")]
        public long IdNumber { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [DisplayName("Full Name")]
        [StringLength(255, ErrorMessage = "Full Name must be 255 characters or fewer")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full Name must contain only letters and spaces")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [StringLength(255, ErrorMessage = "Email Address must be 255 characters or fewer")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [DisplayName("Mobile Number")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile Number must be a 10-digit integer")]
        public long MobileNo { get; set; }

        [Required(ErrorMessage = "Profile Data is required")]
        public ProfileSiteVM ProfileSiteData { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        [StringLength(255, ErrorMessage = "Username must be 255 characters or fewer")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please upload a profile picture.")]
        [DisplayName("User Photo")]
        [MaxFileSize(20 * 1024, ErrorMessage = "Image size cannot exceed 20 KB")]
        public IFormFile PhotoFile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(16, ErrorMessage = "Password must be at most 16 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Is Active is required")]
        [DisplayName("Is Active User")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Is Active Directory User is required")]
        [DisplayName("Is Active Directory User")]
        public bool IsActiveDirectoryUser { get; set; }

        [Required(ErrorMessage = "Change Password is required")]
        [DisplayName("Change Password")]
        public bool ChangePassword { get; set; }

        public int CreateUserId { get; set; }

        public int UpdateUserId { get; set; }
    }
}
