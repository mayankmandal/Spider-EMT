namespace Spider_EMT.Models
{
    public class PageSite
    {
        public int PageId {  get; set; }
        public string PageUrl { get; set; }
        public string PageDescription { get; set; }
        public string MenuImgPath { get; set; }
        public bool isSelected { get; set; }
    }
}
