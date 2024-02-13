using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models
{
    public class TransactionFee
    {
        public int TxnFeeId { get; set; }

        [Required(ErrorMessage = "Withdrawal Fee Amount is required")]
        [DisplayName("Withdrawal Fee Amount")]
        [Range(0,100, ErrorMessage = "Withdrawal Fee Amount must be between 0 and 100")]
        public decimal CWTxnFee { get; set; }

        [Required(ErrorMessage = "Inquiry Fee Amount is required")]
        [DisplayName("Inquiry Fee Amount")]
        [Range(0, 100, ErrorMessage = "Inquiry Fee Amount must be between 0 and 100")]
        public decimal BITxnFee { get; set; }

        [Required(ErrorMessage = "Mini Statement Fee Amount is required")]
        [DisplayName("Mini Statement Fee Amount")]
        [Range(0, 100, ErrorMessage = "Mini Statement Fee Amount must be between 0 and 100")]
        public decimal MSTxnFee { get; set; }

        [Required(ErrorMessage = "Change Pin is required")]
        [DisplayName("Change Pin")]
        [Range(0, 100, ErrorMessage = "Change Pin must be between 0 and 100")]
        public int ChangePin { get; set; } = 0;
    }
}
