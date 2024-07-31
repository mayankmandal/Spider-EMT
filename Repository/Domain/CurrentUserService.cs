using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Spider_EMT.Data.Account;
using Spider_EMT.Pages;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Repository.Domain
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private const string CurrentUserKey = "CurrentUser";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public CurrentUserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<int>> roleManager, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager;
        public SignInManager<ApplicationUser> SignInManager => _signInManager;
        public RoleManager<IdentityRole<int>> RoleManager => _roleManager;

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            if (!_memoryCache.TryGetValue(CurrentUserKey, out ApplicationUser currentUser))
            {
                currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (currentUser != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    _memoryCache.Set(CurrentUserKey, currentUser, cacheEntryOptions);
                }
            }
            return currentUser;
        }

        public async Task<int> GetCurrentUserIdAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }
            return user.Id;
        }

        // Method to refresh the cache on demand
        public async Task RefreshCurrentUserAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                _memoryCache.Set(CurrentUserKey, currentUser, cacheEntryOptions);
            }
        }
    }
}
