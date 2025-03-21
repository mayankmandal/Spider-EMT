﻿using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class ProfileUserVM
    {
        [DisplayName("User ID")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "ID Number is required")]
        [DisplayName("Iqama Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "ID Number must be a 10-digit number")]
        [CheckUniquenessinDB("IdNumber")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [DisplayName("Full Name")]
        [StringLength(200, ErrorMessage = "Full Name must be 200 characters or fewer")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full Name must contain only alphabets and spaces")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [StringLength(100, ErrorMessage = "Email Address must be 100 characters or fewer")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [CheckUniquenessinDB("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [DisplayName("Mobile Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile Number must be a 10-digit number")]
        [CheckUniquenessinDB("MobileNo")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Profile Data is required")]
        public ProfileSiteVM ProfileSiteData { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        [StringLength(100, ErrorMessage = "Username must be 100 characters or fewer")]
        [RegularExpression(@"^[a-zA-Z0-9._ ]*$", ErrorMessage = "Username must contain only alphabets, numbers, spaces, period and underscores")]
        [CheckUniquenessinDB("Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please upload a profile picture.")]
        [DisplayName("User Photo")]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        [MaxFileSize(20 * 1024, ErrorMessage = "Image size cannot exceed 20 KB")]
        public IFormFile PhotoFile { get; set; }

        [DisplayName("New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. Password must be at least 8 characters and at most 16 characters long")]
        public string Password { get; set; }
        [DisplayName("ReType New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. Password must be at least 8 characters and at most 16 characters long")]
        [Compare("Password", ErrorMessage = "New Password and ReType New Password do not match.")]
        public string ReTypePassword { get; set; }

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
