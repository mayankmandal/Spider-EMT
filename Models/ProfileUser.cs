using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Spider_EMT.Utility;

namespace Spider_EMT.Models
{
    public class ProfileUser
    {
        [Required(ErrorMessage = "User ID is required")]
        [DisplayName("User ID")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "ID Number is required")]
        [DisplayName("ID Number")]
        [StringLength(20, ErrorMessage = "ID Number must be 20 characters or fewer")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [DisplayName("Full Name")]
        [StringLength(100, ErrorMessage = "Full Name must be 100 characters or fewer")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [StringLength(50, ErrorMessage = "Email Address must be 50 characters or fewer")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [DisplayName("Mobile Number")]
        [StringLength(15, ErrorMessage = "Mobile Number must be 15 characters or fewer")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Profile Data is required")]
        public ProfileSite ProfileSiteData { get; set; }

        [DisplayName("User Status")]
        public string UserStatus { get; set; }
    }
}
