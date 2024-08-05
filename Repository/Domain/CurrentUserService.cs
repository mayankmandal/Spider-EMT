using LazyCache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Spider_EMT.Repository.Domain
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly System.Net.Http.IHttpClientFactory _clientFactory;

        private const string CurrentUserKey = "CurrentUser";

        public CurrentUserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<int>> roleManager, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IConfiguration configuration, System.Net.Http.IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public HttpContext UserContext => _httpContextAccessor.HttpContext;
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

        public string GenerateJSONWebToken(IEnumerable<Claim> claimsLst)
        {
            string Rephrase = CreateRefreshToken();
            SetRefreshTokenCookie(Rephrase);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings_SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials,
                claims: claimsLst
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void SetJWTCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddHours(3),
                Path = "/"
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(Constants.JwtCookieName, token, cookieOptions);
        }

        public string GetJWTCookie()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[Constants.JwtCookieName];
        }

        public async Task FetchAndCacheUserPermissions(string AccessTokenValue)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessTokenValue);

            // Fetch pages
            var pagesResponse = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserPages");
            var pages = JsonConvert.DeserializeObject<List<PageSiteVM>>(pagesResponse);
            _memoryCache.Set(CacheKeys.CurrentUserPagesKey, pages, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                SlidingExpiration = TimeSpan.FromSeconds(10),
                Size = 1024
            });

            // Fetch categories
            var categoriesResponse = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserCategories");
            var categories = JsonConvert.DeserializeObject<List<CategoryDisplayViewModel>>(categoriesResponse);
            _memoryCache.Set(CacheKeys.CurrentUserCategoriesKey, categories, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                SlidingExpiration = TimeSpan.FromSeconds(10),
                Size = 1024
            });
        }

        private string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                string token = Convert.ToBase64String(randomNumber);
                return token;
            }
        }
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly= true,
                Expires = DateTime.Now.AddDays(7),
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(Constants.JwtRefreshTokenName, refreshToken, cookieOptions);
        }
    }
}
