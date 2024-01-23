using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Spider_EMT.Models.ViewModels
{
    public class ChartsViewModel
    {
        [Required(ErrorMessage = "From Date is required")]
        [DisplayName("From Date")]
        public DateTime FromDate {  get; set; }
        [Required(ErrorMessage = "To Date is required")]
        [DisplayName("To Date")]
        public DateTime ToDate {  get; set; }
        public string TransactionAmountType { get; set; }
        public IList<ChartTransactionData> ChartTransactionDataList { get; set; }
    }
    public class ChartTransactionData
    {
        public decimal AverageAmount { get; set; }
        public string BankNameEn { get; set; }
        public string BankShortName { get; set; }
    }
}
