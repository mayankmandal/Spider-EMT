using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
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
        public async Task<IActionResult> OnGetAsync(bool rememberMe)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                TempData["error"] = "User not found.";
                ModelState.AddModelError("AuthenticatorSetup", "User not found.");
                return RedirectToPage("/Account/Login");
            }
            if (!user.EmailConfirmed)
            {
                TempData["error"] = "Please confirm your email before setting up two-factor authentication.";
                return RedirectToPage("/Account/Login");
            }
            if (user.EmailConfirmed && !user.TwoFactorEnabled)
            {
                return RedirectToPage("/Account/AuthenticatorWithMFASetup");
            }

            AuthenticatorMFAData.SecurityCode = string.Empty;
            AuthenticatorMFAData.RememberMe = rememberMe;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                TempData["error"] = "Invalid input. Please check your data and try again.";
                return Page();
            }

            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                TempData["error"] = "User not found.";
                ModelState.AddModelError("AuthenticatorSetup", "User not found.");
                return RedirectToPage("/Account/Login");
            }
            if (!user.EmailConfirmed)
            {
                TempData["error"] = "Please confirm your email before setting up two-factor authentication.";
                return RedirectToPage("/Account/Login");
            }
            if (user.EmailConfirmed && !user.TwoFactorEnabled)
            {
                return RedirectToPage("/Account/AuthenticatorWithMFASetup");
            }

            var result = await _currentUserService.SignInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFAData.SecurityCode, AuthenticatorMFAData.RememberMe, false);

            if (result.Succeeded)
            {
                _currentUserService.RefreshCurrentUserAsync();
                if (user.UserVerificationSetupEnabled != true)
                {
                    TempData["success"] = "Please complete your user verification setup.";
                    return RedirectToPage("/Account/UserVerificationSetup");
                }
                else if(user.UserVerificationSetupEnabled == true && user.RoleAssignmentEnabled == false)
                {
                    TempData["success"] = "Firstly User need to be assigned with Appropriate Role for further accessibility";
                    return RedirectToPage("/Account/UserRoleAssignment");
                }
                else if(user.UserVerificationSetupEnabled == true && user.RoleAssignmentEnabled == true)
                {
                    TempData["success"] = "Login successful.";
                    return RedirectToPage("/Dashboard");
                }
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
            }
            return Page();
        }
    }
}
