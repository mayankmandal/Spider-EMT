using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Data.Account;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFAData { get; set; }
        public LoginTwoFactorWithAuthenticatorModel(ICurrentUserService currentUserService)
        {
            AuthenticatorMFAData = new AuthenticatorMFAViewModel();
            _currentUserService = currentUserService;
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
                TempData["error"] = "Invalid input. Please check your data and try again.";
                return Page();
            }

            var result = await _currentUserService.SignInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFAData.SecurityCode, AuthenticatorMFAData.RememberMe, false);

            if (result.Succeeded)
            {
                /*var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    TempData["error"] = "User not found.";
                    ModelState.AddModelError("Authenticator2FA", "User not found.");
                    return Page();
                }

                if ((bool)!user.UserVerificationSetupEnabled)
                {
                    TempData["success"] = "Please complete your user verification setup.";
                    return RedirectToPage("/Account/UserVerificationSetup");
                }*/
                TempData["success"] = "Login successful.";
                return RedirectToPage("/Dashboard");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    TempData["error"] = "You are locked out!";
                    ModelState.AddModelError("Authenticator2FA", "You are locked out!");
                }
                else
                {
                    TempData["error"] = "Failed to login.";
                    ModelState.AddModelError("Authenticator2FA", "Failed to Login.");
                }
                return Page();
            }
        }
    }
}
