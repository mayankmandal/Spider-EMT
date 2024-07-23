using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Configuration.Authorization.Models
{
    public class SetupMFAViewModel
    {
        public string Key { get; set; }
        [Required]
        [DisplayName("Authenticator Code")]
        public string SecurityCode { get; set; }
        [DisplayName("QR Code to Scan")]
        public Byte[] QRCodeBytes { get; set; }
    }
}
