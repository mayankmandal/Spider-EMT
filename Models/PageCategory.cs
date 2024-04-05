namespace Spider_EMT.Models
{
    public class PageCategory
    {
        public int PageCatId { get; set; }
        public string CategoryName { get; set; }
        // Navigation property for Pages
        public ICollection<Page> Pages { get; set; }
    }
}
