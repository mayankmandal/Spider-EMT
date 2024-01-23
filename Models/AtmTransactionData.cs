namespace Spider_EMT.Models
{
    public class AtmTransactionData
    {
        public long TxnId { get; set; }
        public string TermId { get; set; }
        public DateTime TxnDate { get; set; }
        public int TotalCWCount { get; set; }
        public int TotalBICount { get; set; }
        public int TotalMSCount { get; set; }
        public decimal TotalCWAmount { get; set; }
    }
}
