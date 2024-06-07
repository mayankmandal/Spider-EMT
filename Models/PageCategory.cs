using System.ComponentModel;

namespace Spider_EMT.Models
{
    public class PageCategory
    {
        public int PageCatId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        // Navigation property for Pages
        public int PageId {  get; set; }
    }
}
