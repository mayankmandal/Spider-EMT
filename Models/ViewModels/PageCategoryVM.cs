using System.ComponentModel;

namespace Spider_EMT.Models.ViewModels
{
    public class PageCategoryVM
    {
        public int PageCatId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        // Navigation property for Pages
        public int PageId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}
