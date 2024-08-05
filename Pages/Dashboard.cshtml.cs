using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
    public class DashboardModel : PageModel
    {

        public void OnGet()
        {
        }
    }
}
