using Microsoft.AspNetCore.Mvc.ModelBinding;
using Spider_EMT.Models.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class SettingsAPIVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhotoFile { get; set; } = string.Empty;
        public string SettingsPassword {  get; set; } = string.Empty;
        public string SettingsReTypePassword {  get; set; } = string.Empty;
    }
}
