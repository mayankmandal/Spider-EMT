using Microsoft.AspNetCore.Identity;
using Spider_EMT.Data.Account;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ICurrentUserService
    {
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<int> GetCurrentUserIdAsync();
        Task RefreshCurrentUserAsync();
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        RoleManager<IdentityRole<int>> RoleManager { get; }
    }
}
