﻿using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class ProfileSiteVM
    {
        [Required(ErrorMessage = "Profile Id is required")]
        [DisplayName("Profile Id")]
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Profile Name is required")]
        [DisplayName("Profile Name")]
        [RegularExpression(@"^[a-zA-Z0-9_\s]*$", ErrorMessage = "Profile Name can only contain alphabets, numbers, whitespaces and underscore.")]
        [StringLength(100, ErrorMessage = "Profile Name must be 100 characters or fewer")]
        [CheckUniquenessinDB("name")]
        public string ProfileName { get; set; }
    }
}
