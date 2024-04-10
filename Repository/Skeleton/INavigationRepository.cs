using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        List<ProfileSite> GetAllProfiles();
        List<PageSite> GetAllPages();
        List<PageCategory> GetAllPageCategories();
        List<CurrentUserProfileViewModel> GetCurrentProfiles();
        Task<bool> AddUserProfile(ProfileSite profile, List<PageSite> pages, List<PageCategory> pageCategories);
        Task<bool> AddUserPermissions(UserPermission userPermission);
        List<PageCategory> GetPageToCategories(List<int> pageList);
    }
}
