using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Configuration.Authorization.Models
{
    public class EmailMFA
    {
        [Required]
        [DisplayName("Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
