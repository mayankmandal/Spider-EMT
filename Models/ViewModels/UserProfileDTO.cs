namespace Spider_EMT.Models.ViewModels
{
    public class UserProfileDTO
    {
        public ProfileSite Profile { get; set; }
        public List<PageSite> Pages { get; set; }
        public List<PageCategory> PageCategories { get; set; }
    }
}
