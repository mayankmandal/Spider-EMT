using Microsoft.AspNetCore.Identity;
using Spider_EMT.Data.Account;
using System.Security.Claims;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ICurrentUserService
    {
        Task<ApplicationUser> GetCurrentUserAsync(ApplicationUser? user = null);
        Task<int> GetCurrentUserIdAsync();
        Task RefreshCurrentUserAsync();
        Task<UserRoleInfo> GetCurrentUserRolesAsync(string jwtToken);
        UserManager<ApplicationUser> UserManager { get; }
        SignInManager<ApplicationUser> SignInManager { get; }
        RoleManager<IdentityRole<int>> RoleManager { get; }
        string GenerateJSONWebToken(IEnumerable<Claim> claims);
        void SetJWTCookie(string token, string name);
        string GetJWTCookie(string name);
        HttpContext UserContext { get; }
        Task FetchAndCacheUserPermissions(string AccessTokenValue);
        Task<IList<Claim>> GetCurrentUserClaimsAsync(ApplicationUser user);
        Task<ApplicationUser> GetCurrentUserAsync(string jwtToken);
        Task<int> GetCurrentUserIdAsync(string jwtToken = null);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<UserRoleInfo> GetUserRoleAsync(string email);
    }
}
