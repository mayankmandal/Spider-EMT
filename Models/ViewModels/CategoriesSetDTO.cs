namespace Spider_EMT.Models.ViewModels
{
    public class CategoriesSetDTO
    {
        public int PageCatId { get; set; }
        public string CatagoryName { get; set; }
        public int PageId { get; set; }
        public string PageDescription { get; set; }
        public string PageUrl { get; set; }
        public string MenuImgPath { get; set; }

    }

    public class CategoryDisplayViewModel
    {
        public string CategoryName { get; set; }
        public List<PageDisplayViewModel> Pages { get; set; }
    }

    public class PageDisplayViewModel
    {
        public string PageDescription { get; set; }
        public string PageUrl { get; set; }
    }

}
