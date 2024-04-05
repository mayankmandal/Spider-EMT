using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface INavigationRepository
    {
        List<Profile> GetAllProfiles();
        List<Page> GetAllPages();
        List<PageCategory> GetAllPageCategories();
        List<CurrentUserProfileViewModel> GetCurrentProfiles();
        Task<bool> AddUserProfile(Profile profile);
        Task<bool> AddUserPermissions(UserPermission userPermission);
    }
}
