using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
    public class LogoutModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        public LogoutModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            await _currentUserService.SignInManager.SignOutAsync();

            // Clear the JWT token cookie
            Response.Cookies.Delete(Constants.JwtCookieName);
            Response.Cookies.Delete(Constants.JwtAMRTokenName);

            // Invalidate the session
            HttpContext.Session.Clear();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/Account/Login");
            }
        }
    }
}
