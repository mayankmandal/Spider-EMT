using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Data.Account;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        public LogoutModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _currentUserService.SignInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}
