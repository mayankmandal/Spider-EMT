using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Spider_EMT.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public LoginModel(IConfiguration configuration, UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [BindProperty]
        public CredentialViewModel? CredentialData { get; set; }
        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGet()
        {
            ExternalLoginProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(CredentialData.Email, CredentialData.Password, CredentialData.RememberMe, false);

            if (result.Succeeded)
            {
                /*var user = await _userManager.FindByEmailAsync(CredentialData.Email);
                var token = GenerateJwtToken(user);

                // Set the token in a cookie
                Response.Cookies.Append("JwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });*/

                return RedirectToPage("/Dashboard");
            }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new
                    {
                        RememberMe = CredentialData.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out!");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to Login.");
                }
                return Page();
            }
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider,null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(properties, provider);
        }
        private string GenerateJwtToken(ApplicationUser user)
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
        }
    }
}
