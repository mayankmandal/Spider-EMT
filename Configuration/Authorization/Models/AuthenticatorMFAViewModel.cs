using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Configuration.Authorization.Models
{
    public class AuthenticatorMFAViewModel
    {
        [Required]
        [DisplayName("Authenticator Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
