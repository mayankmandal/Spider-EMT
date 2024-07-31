using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        Task<List<ProfileUserAPIVM>> GetAllUsersDataAsync();
        Task<List<ProfileSite>> GetAllProfilesAsync();
        Task<List<PageSite>> GetAllPagesAsync();
        Task<List<PageCategory>> GetAllCategoriesAsync();
        Task<ProfileUserAPIVM> GetCurrentUserDetailsAsync(int CurrentUserId);
        Task<ProfileSite> GetCurrentUserProfileAsync(int CurrentUserId);
        Task<List<PageSiteVM>> GetCurrentUserPagesAsync(int CurrentUserId);
        Task<List<PageSite>> GetProfilePageDataAsync(string profileId);
        Task<List<CategoriesSetDTO>> GetCurrentUserCategoriesAsync(int CurrentUserId);
        Task<List<PageCategory>> GetPageToCategoriesAsync(List<int> pageList);
        Task<List<PageSite>> GetCategoryToPagesAsync(int categoryId);
        Task<bool> CreateUserProfileAsync(ProfileUser profileUsersData, int CurrentUserId);
        Task<string> UpdateUserProfileAsync(ProfileUserAPIVM profileUsersData, int CurrentUserId);
        Task<bool> CreateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO, int CurrentUserId);
        Task<bool> CreateNewCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO, int CurrentUserId);
        Task<bool> UpdateCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO, int CurrentUserId);
        Task<bool> UpdateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO, int CurrentUserId);
        Task<bool> AssignProfileCategoriesAsync(ProfileCategoryAccessDTO profileCategoryAccessDTO, int CurrentUserId);
        Task<List<ProfileUserAPIVM>> SearchUserDetailsByCriteriaAsync(string criteriaText, string inputText);
        Task<bool> DeleteEntityAsync(int deleteId, string deleteType);
        Task<ProfileUserAPIVM> GetSettingsDataAsync(int CurrentUserId);
        Task<string> UpdateSettingsDataAsync(ProfileUser userSettings, int CurrentUserId);
        Task<bool> CheckUniquenessAsync(string field, string value);
        Task<ProfileUserAPIVM> GetUserRecordAsync(int userId);
        Task<string> UpdateUserVerificationAsync(UserVerifyApiVM userVerifyApiVM, int CurrentUserId);
    }
}
