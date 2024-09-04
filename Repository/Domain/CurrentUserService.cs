using Microsoft.AspNetCore.Identity;
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
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IHttpClientFactory _clientFactory;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<int>> roleManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
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
            if (!_httpContextAccessor.HttpContext.Session.TryGetValue(SessionKeys.CurrentUserKey, out byte[] currentUserData))
            {
                // Session key not found, fetch user data
                if (user == null)
                {
                    // Retrieve JWT or other identifier
                    var token = GetJWTCookie(Constants.JwtCookieName);
                    if (!string.IsNullOrEmpty(token))
                    {
                        var principal = GetPrincipalFromToken(token);
                        var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        if (!string.IsNullOrEmpty(emailClaim))
                        {
                            var currentUser = await _userManager.FindByEmailAsync(emailClaim);
                            if (currentUser != null)
                            {
                                _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentUser)));
                                return currentUser;
                            }
                        }
                    }
                    return null; // No user found, return null
                }
                else
                {
                    // Set the session with the provided user
                    _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user)));
                }
            }
            else
            {
                // Retrieve and deserialize the user data from the session
                user = JsonConvert.DeserializeObject<ApplicationUser>(Encoding.UTF8.GetString(currentUserData));
            }
            return user;
        }
        
        public async Task<ApplicationUser> GetCurrentUserAsync(string jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var principal = GetPrincipalFromToken(jwtToken);
                var emailClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(emailClaim))
                {
                    return await _userManager.FindByEmailAsync(emailClaim);
                }
            }
            return null; // Return null if no user found
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

        public async Task<int> GetCurrentUserIdAsync(string? jwtToken = null)
        {
            var user = await GetCurrentUserAsync(jwtToken);
            if (user == null)
            {
                throw new InvalidOperationException("User is not authenticated");
            }
            return user.Id;
        }

        // Method to refresh the cache on demand
        public async Task RefreshCurrentUserAsync()
        {
            _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserKey);
        }

        public async Task<UserRoleInfo> GetCurrentUserRolesAsync(string jwtToken)
        {
            var user = await GetCurrentUserAsync(jwtToken);
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

        public void SetJWTCookie(string token, string name)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ensure cookies are sent only over HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(3) // Token expiration time
                };
                // _httpContextAccessor.HttpContext.Session.SetString(name, token);  // Store token in session
                _httpContextAccessor.HttpContext.Response.Cookies.Append(name, token, cookieOptions);
            }
            else
            {
                throw new InvalidOperationException("HttpContext is null. Cannot set JWT cookie.");
            }
        }

        public string GetJWTCookie(string name)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                return _httpContextAccessor.HttpContext.Request.Cookies[name];  // Retrieve token from session
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

            _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserPagesKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pages)));

            // Fetch categories
            var categoriesResponse = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserCategories");
            var categories = JsonConvert.DeserializeObject<List<CategoryDisplayViewModel>>(categoriesResponse);
            _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserCategoriesKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(categories)));
        }

        public async Task<IList<Claim>> GetCurrentUserClaimsAsync(ApplicationUser user)
        {
            
            if (!_httpContextAccessor.HttpContext.Session.TryGetValue(SessionKeys.CurrentUserClaimsKey, out byte[] claimsData))
            {
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var roles = await _userManager.GetRolesAsync(user);
                    foreach(var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserClaimsKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(claims)));
                    await GetCurrentUserAsync(user);

                    return claims;
                }
            }
            else
            {
                JsonConvert.DeserializeObject<IList<Claim>>(Encoding.UTF8.GetString(claimsData));
            }
            return null;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"No user found with the email: {email}");
            }
            return user;
        }

        public async Task<UserRoleInfo> GetUserRoleAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException("User is not found.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if(roles == null || roles.Count == 0)
            {
                throw new InvalidOperationException("User does not have any roles assigned.");
            }
            var roleName = roles.First();
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Role not found.");
            }
            return new UserRoleInfo
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
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

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings_SecretKey"])),
                ClockSkew = TimeSpan.Zero // Remove any tolerance for expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;


            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch
            {
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
