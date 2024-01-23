namespace Spider_EMT.Models.ViewModels
{
    public class BankTransactionSummaryViewModel
    {
        public string BankNameEn { get; set; }
        public string BankLogoPath { get; set; }
        public string TermId { get; set; }
        public string RegionEn { get; set; }
        public string CityEn { get; set; }
        public DateTime TxnDate { get; set; }
        public int TotalCWCount { get; set; }
        public decimal TotalCWFeeAmount { get; set; }
        public int TotalBICount { get; set; }
        public int TotalMSCount { get; set; }
        public decimal TotalBI_MSFeeAmount { get; set; }
        public int TotalTxnOnUsCount { get; set; }
        public decimal TotalPayedAmount { get; set; }
    }
}
