using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        Task<CurrentUser> GetCurrentUserAsync();
        Task<List<ProfileUserAPIVM>> GetAllUsersDataAsync();
        Task<List<ProfileSite>> GetAllProfilesAsync();
        Task<List<PageSite>> GetAllPagesAsync();
        Task<List<PageCategory>> GetAllCategoriesAsync();
        Task<ProfileUserAPIVM> GetCurrentUserDetailsAsync();
        Task<ProfileSite> GetCurrentUserProfileAsync();
        Task<List<PageSiteVM>> GetCurrentUserPagesAsync();
        Task<List<PageSite>> GetProfilePageDataAsync(string profileId);
        Task<List<CategoriesSetDTO>> GetCurrentUserCategoriesAsync();
        Task<List<PageCategory>> GetPageToCategoriesAsync(List<int> pageList);
        Task<List<PageSite>> GetCategoryToPagesAsync(int categoryId);
        Task<bool> CreateUserProfileAsync(ProfileUser profileUsersData);
        Task<string> UpdateUserProfileAsync(ProfileUserAPIVM profileUsersData);
        Task<bool> CreateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> CreateNewCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO);
        Task<bool> UpdateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO);
        Task<bool> AssignProfileCategoriesAsync(ProfileCategoryAccessDTO profileCategoryAccessDTO);
        Task<List<ProfileUserAPIVM>> SearchUserDetailsByCriteriaAsync(string criteriaText, string inputText);
        Task<bool> DeleteEntityAsync(int deleteId, string deleteType);
        Task<ProfileUserAPIVM> GetSettingsDataAsync();
        Task<string> UpdateSettingsDataAsync(ProfileUser userSettings);
        Task<bool> CheckUniquenessAsync(string field, string value);
        Task<ProfileUserAPIVM> GetUserRecordAsync(int userId);
    }
}
