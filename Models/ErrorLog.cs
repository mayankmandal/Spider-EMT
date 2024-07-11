namespace Spider_EMT.Models
{
    public class ErrorLog
    {
        public int LogId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? CreateUserId { get; set; }
        public int? UpdateUserId { get; set; }
    }
}
