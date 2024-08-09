using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
    public class DashboardModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        public DashboardModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null ||
                user.EmailConfirmed != true ||
                user.TwoFactorEnabled != true ||
                user.UserVerificationSetupEnabled != true ||
                user.RoleAssignmentEnabled != true)
            {
                TempData["error"] = $"Invalid login attempt. Please check your input and try again.";
                return RedirectToPage("/Account/Login");
            }
            return Page();
        }
    }
}
