using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Security.Claims;

namespace Spider_EMT.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;

        public LoginModel(IConfiguration configuration, ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
            
        }
        [BindProperty]
        public CredentialViewModel? CredentialData { get; set; }
        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGet()
        {
            ExternalLoginProviders = await _currentUserService.SignInManager.GetExternalAuthenticationSchemesAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ExternalLoginProviders = await _currentUserService.SignInManager.GetExternalAuthenticationSchemesAsync();
            if (!ModelState.IsValid)
            {
                TempData["error"] = $"Invalid login attempt. Please check your input and try again.";
                return Page();
            }

            var user = await _currentUserService.UserManager.FindByEmailAsync(CredentialData.Email);
            if (user == null)
            {
                TempData["error"] = $"Invalid login attempt. Please check your input and try again.";
                ModelState.AddModelError("Login", "Invalid login attempt.");
                return Page();
            }

            if (!user.EmailConfirmed)
            {
                TempData["error"] = $"Email is not confirmed for {user.Email}. Please confirm your email.";
                ModelState.AddModelError("Login", "You must have a confirmed email to log in.");
                return Page();
            }

            var result = await _currentUserService.SignInManager.PasswordSignInAsync(CredentialData.Email, CredentialData.Password, CredentialData.RememberMe, false);

            /*// Extra Temp Login for direct Login
            if (result.Succeeded)
            {
                // Redirect to the dashboard page after successful login
                return RedirectToPage("/Dashboard");
            }*/

            if (result.Succeeded && !result.RequiresTwoFactor)
            {
                TempData["success"] = $"{user.Email} - Login successful.";
                return RedirectToPage("/Account/AuthenticatorWithMFASetup");
            }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    // Role and claim management
                    await AddUserRolesAndClaims(user);

                    TempData["success"] = $"{user.Email} - Login successful. Please complete two-factor authentication";
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new
                    {
                        RememberMe = CredentialData.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    TempData["error"] = $"{user.Email} - Your account is locked out!";
                    ModelState.AddModelError("Login", "You are locked out!");
                }
                else
                {
                    TempData["error"] = $"{user.Email} - Failed to login.";
                    ModelState.AddModelError("Login", "Failed to Login.");
                }
                return Page();
            }
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = _currentUserService.SignInManager.ConfigureExternalAuthenticationProperties(provider,null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");
            TempData["success"] = $"External login initiated for {provider}.";
            return Challenge(properties, provider);
        }
        /*private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/

        private async Task AddUserRolesAndClaims(ApplicationUser user)
        {
            // Ensure roles are required
            var roles = await _currentUserService.UserManager.GetRolesAsync(user);
            foreach(var roleName in roles)
            {
                if(!await _currentUserService.RoleManager.RoleExistsAsync(roleName))
                {
                    // Create the role if it doesn't exist
                    var role = new IdentityRole<int> { Name = roleName };
                    await _currentUserService.RoleManager.CreateAsync(role);
                }
            }

            // Add Claims based on roles
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await _currentUserService.UserManager.GetClaimsAsync(user);
            foreach(var claim in claims)
            {
                if (!userClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
                {
                    await _currentUserService.UserManager.AddClaimAsync(user, claim);
                }
            }
        }
    }
}
