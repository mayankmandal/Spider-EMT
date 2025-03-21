﻿using Microsoft.AspNetCore.Identity;

namespace Spider_EMT.Data.Account
{
    public class ApplicationUser : IdentityUser<int>, IAudittable
    {
        public string? IdNumber { get; set; }
        public string? FullName { get; set; }
        public string? MobileNo { get; set; }
        public string? Userimgpath { get; set; }
        public bool? UserVerificationSetupEnabled { get; set; }
        public bool? RoleAssignmentEnabled { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsActiveDirectoryUser { get; set; }
        public bool? ChangePassword { get; set; }
        public DateTime? LastLoginActivity { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserId { get; set; }
    }
}
