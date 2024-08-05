using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
    public class AccessDeniedModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string returnURL)
        {
            ViewData["ReturnUrl"] = returnURL;
            TempData["error"] = $"You do not have permission to access the page: {returnURL}";
            return Page();
        }
    }
}
