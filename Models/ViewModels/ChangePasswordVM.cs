using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class ChangePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. Password must be at least 8 characters and at most 16 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. Password must be at least 8 characters and at most 16 characters long")]
        [Compare("NewPassword", ErrorMessage = "The New password and Confirmation Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
