using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        CurrentUser GetCurrentUser();
        List<ProfileUser> GetAllProfileUsers();
        List<ProfileSite> GetAllProfiles();
        List<PageSite> GetAllPages();
        List<PageCategory> GetAllPageCategories();
        ProfileSite GetCurrentUserProfile();
        List<PageSite> GetCurrentUserPages();
        List<PageSite> GetNewUserPages();
        List<PageCategory> GetCurrentUserCategories();
        List<PageCategory> GetNewUserCategories();
        Task<bool> AddUserProfile(ProfileSite profile, List<PageSite> pages, List<PageCategory> pageCategories);
        Task<bool> AddNewUserProfile(List<ProfileSite> profiles, List<ProfileUser> profileUsers);
        Task<bool> UpdateUserProfile(ProfileSite profile, List<PageSite> pages, List<PageCategory> pageCategories);
        List<PageCategory> GetPageToCategories(List<int> pageList);
    }
}
