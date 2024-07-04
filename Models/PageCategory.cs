namespace Spider_EMT.Models
{
    public class PageCategory
    {
        public int PageCatId { get; set; }
        public string CategoryName { get; set; }
        // Navigation property for Pages
        public int PageId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
