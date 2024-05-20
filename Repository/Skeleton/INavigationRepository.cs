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
        List<PageCategory> GetAllCategories();
        ProfileUser GetCurrentUserDetails();
        ProfileSite GetCurrentUserProfile();
        List<PageSite> GetCurrentUserPages();
        List<PageSite> GetProfilePageData(string profileId);
        List<PageSite> GetNewUserPages();
        List<CategoriesSetDTO> GetCurrentUserCategories();
        List<PageCategory> GetNewUserCategories();
        List<PageCategory> GetPageToCategories(List<int> pageList);
        List<PageSite> GetCategoryToPages(int categoryId);
        Task<bool> CreateUserProfile(ProfileUser profileUsersData);
        Task<bool> CreateUserAccess(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> CreateNewCategory(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateCategory(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateUserAccess(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> AssignProfileCategories(ProfileCategoryAccessDTO profileCategoryAccessDTO);
    }
}
