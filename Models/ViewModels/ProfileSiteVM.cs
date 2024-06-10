using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Spider_EMT.Models.ViewModels
{
    public class ProfileSiteVM
    {
        [Required(ErrorMessage = "Profile Id is required")]
        [DisplayName("Profile Id")]
        public int ProfileId { get; set; }
        [Required(ErrorMessage = "Profile Name is required")]
        [DisplayName("Profile Name")]
        [StringLength(100, ErrorMessage = "Profile Name must be 100 characters or fewer")]
        public string ProfileName { get; set; }
    }
}
