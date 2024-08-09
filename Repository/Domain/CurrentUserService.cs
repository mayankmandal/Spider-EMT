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
        private readonly IHttpClientFactory _clientFactory;

        private const string CurrentUserKey = "CurrentUser";
        private const string CurrentUserClaimsKey = "CurrentUserClaims";

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<int>> roleManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _clientFactory = httpClientFactory;
        }

        public HttpContext UserContext => _httpContextAccessor.HttpContext;
        public UserManager<ApplicationUser> UserManager => _userManager;
        public SignInManager<ApplicationUser> SignInManager => _signInManager;
        public RoleManager<IdentityRole<int>> RoleManager => _roleManager;

        public async Task<ApplicationUser> GetCurrentUserAsync(ApplicationUser? user = null)
        {
            if (!_memoryCache.TryGetValue(CurrentUserKey, out ApplicationUser currentUser))
            {
                if (user == null)
                {
                    var token = GetJWTCookie();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var principal = GetPrincipalFromToken(token);
                        var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        if (!string.IsNullOrEmpty(emailClaim))
                        {
                            currentUser = await _userManager.FindByEmailAsync(emailClaim);
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
                else
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };
                    _memoryCache.Set(CurrentUserKey, user, cacheEntryOptions);
                    currentUser = user;
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
            _memoryCache.Remove(CurrentUserKey);
            await GetCurrentUserAsync();
        }

        public async Task<UserRoleInfo> GetCurrentUserRolesAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if(roles == null || roles.Count == 0)
            {
                throw new InvalidOperationException("User does not have a role assigned");
            }

            var roleName = roles.First();
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Role not found");
            }

            return new UserRoleInfo
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
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
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public void SetJWTCookie(string token)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString(Constants.JwtCookieName, token);  // Store token in session
            }
            else
            {
                throw new InvalidOperationException("HttpContext is null. Cannot set JWT cookie.");
            }
        }

        public string GetJWTCookie()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var session = _httpContextAccessor.HttpContext.Session;
                return session.GetString(Constants.JwtCookieName);  // Retrieve token from session
            }
            else
            {
                throw new InvalidOperationException("HttpContext is null. Cannot get JWT cookie.");
            }
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

        public async Task<IList<Claim>> GetCurrentUserClaimsAsync(ApplicationUser user)
        {
            if (!_memoryCache.TryGetValue(CurrentUserClaimsKey, out IList<Claim> claims))
            {
                if (user != null)
                {
                    claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var roles = await _userManager.GetRolesAsync(user);
                    foreach(var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    };
                    _memoryCache.Set(CurrentUserClaimsKey, claims, cacheEntryOptions);

                    await GetCurrentUserAsync(user);
                }
            }
            return claims;
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

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings_SecretKey"]))
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            var jwtToken = securityToken as JwtSecurityToken;
            if(jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
