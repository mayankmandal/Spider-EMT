namespace Spider_EMT.Models
{
    public class Page
    {
        public int PageId {  get; set; }
        public string PageUrl { get; set; }
        public string PageDescription { get; set; }
        public int PageCatId { get; set; }
        public string MenuImgPath { get; set; }
        public PageCategory PageCategory { get; set; }
        // Navigation property for UserPermission
        public ICollection<UserPermission> UserPermissions { get; set; }
    }
}
