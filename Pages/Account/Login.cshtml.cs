using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;

namespace Spider_EMT.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        public LoginModel(ICurrentUserService currentUserService)
        {
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

            await _currentUserService.SignInManager.SignOutAsync();
            var result = await _currentUserService.SignInManager.PasswordSignInAsync(user.Email, CredentialData.Password, CredentialData.RememberMe, false);
            if (result.Succeeded)
            {
                if (user.EmailConfirmed && user.TwoFactorEnabled)
                {
                    await ManageUserClaimsAndPermissions(user);
                    return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new { RememberMe = CredentialData.RememberMe });
                }

                if (result.Succeeded && !result.RequiresTwoFactor)
                {
                    TempData["success"] = $"{user.Email} - Login successful.";
                    await ManageUserClaimsAndPermissions(user);
                    if (user.TwoFactorEnabled)
                    {
                        TempData["success"] = $"{user.Email} - Login successful. Please complete two-factor authentication";
                        return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new { RememberMe = CredentialData.RememberMe });
                    }
                    return RedirectToPage("/Account/AuthenticatorWithMFASetup");
                }
            }
            else
            {
                if (result.RequiresTwoFactor)
                {
                    await ManageUserClaimsAndPermissions(user);
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
            }
            return Page();
        }
        public IActionResult OnPostLoginExternally(string provider)
        {
            var properties = _currentUserService.SignInManager.ConfigureExternalAuthenticationProperties(provider,null);
            properties.RedirectUri = Url.Action("ExternalLoginCallback", "Account");
            TempData["success"] = $"External login initiated for {provider}.";
            return Challenge(properties, provider);
        }
        private async Task ManageUserClaimsAndPermissions(ApplicationUser user)
        {
            // Role and claim management
            var claims = await _currentUserService.GetCurrentUserClaimsAsync(user);
            var accessToken = _currentUserService.GenerateJSONWebToken(claims);
            _currentUserService.SetJWTCookie(accessToken, Constants.JwtCookieName);

            // Fetch and cache user permissions from API
            // await _currentUserService.FetchAndCacheUserPermissions(accessToken);
        }
    }
}
