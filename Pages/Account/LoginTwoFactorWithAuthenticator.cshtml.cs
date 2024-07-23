using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Data.Account;

namespace Spider_EMT.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFAData { get; set; }
        public LoginTwoFactorWithAuthenticatorModel(SignInManager<ApplicationUser> signInManager)
        {
            AuthenticatorMFAData = new AuthenticatorMFAViewModel();
            _signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFAData.SecurityCode = string.Empty;
            AuthenticatorMFAData.RememberMe = rememberMe;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFAData.SecurityCode, AuthenticatorMFAData.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Dashboard");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "You are locked out!");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Failed to Login.");
                }
                return Page();
            }
        }
    }
}
