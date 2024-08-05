using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Data.Account;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        [BindProperty]
        public string Message { get; set; }
        public ConfirmEmailModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["error"] = "Invalid email confirmation request.";
                return RedirectToPage("/Account/Login");
            }

            var user = await _currentUserService.UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["error"] = "Invalid user.";
                return RedirectToPage("/Account/Login");
            }

            var result = await _currentUserService.UserManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Sign out the user after successful email confirmation
                await _currentUserService.SignInManager.SignOutAsync();

                // Set TempData for success message
                TempData["success"] = "Email address is successfully confirmed. You can now log in.";

                return RedirectToPage("/Account/Login");
            }
            else
            {
                // Set TempData for error message
                TempData["error"] = "Failed to validate email.";
                return Page();
            }
        }
    }
}
