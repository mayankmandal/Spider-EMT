namespace Spider_EMT.Models
{
    public class PageSite
    {
        public int PageId {  get; set; }
        public string PageUrl { get; set; }
        public string PageDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
        public bool isSelected { get; set; }
    }
}
