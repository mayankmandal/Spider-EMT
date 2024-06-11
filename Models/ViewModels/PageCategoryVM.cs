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
    }
}
