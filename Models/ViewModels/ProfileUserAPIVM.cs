using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Spider_EMT.Models.ViewModels
{
    public class ProfileUserAPIVM
    {
        public int UserId { get; set; }
        public long IdNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public long MobileNo { get; set; }
        public ProfileSiteVM ProfileSiteData { get; set; }
        public string Username { get; set; }
        public string Userimgpath { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsActiveDirectoryUser { get; set; }
        public bool ChangePassword { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }
    }
}
