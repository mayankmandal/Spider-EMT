using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        Task<CurrentUser> GetCurrentUserAsync();
        Task<List<ProfileUser>> GetAllUsersDataAsync();
        Task<List<ProfileSite>> GetAllProfilesAsync();
        Task<List<PageSite>> GetAllPagesAsync();
        Task<List<PageCategory>> GetAllCategoriesAsync();
        Task<ProfileUser> GetCurrentUserDetailsAsync();
        Task<ProfileSite> GetCurrentUserProfileAsync();
        Task<List<PageSite>> GetCurrentUserPagesAsync();
        Task<List<PageSite>> GetProfilePageDataAsync(string profileId);
        Task<List<CategoriesSetDTO>> GetCurrentUserCategoriesAsync();
        Task<List<PageCategory>> GetPageToCategoriesAsync(List<int> pageList);
        Task<List<PageSite>> GetCategoryToPagesAsync(int categoryId);
        Task<bool> CreateUserProfileAsync(ProfileUser profileUsersData);
        Task<bool> UpdateUserProfileAsync(ProfileUser profileUsersData);
        Task<bool> CreateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> CreateNewCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> AssignProfileCategoriesAsync(ProfileCategoryAccessDTO profileCategoryAccessDTO);
        Task<List<ProfileUserAPIVM>> SearchUserDetailsByCriteriaAsync(string criteriaText, string inputText);
        Task<bool> DeleteEntityAsync(int deleteId, string deleteType);
        Task<UserSettings> GetSettingsDataAsync();
        Task<string> UpdateSettingsDataAsync(UserSettings userSettings);
    }
}
