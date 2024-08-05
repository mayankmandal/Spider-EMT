using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
    public class ErrorModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int LogId { get; set; }
        public void OnGet()
        {
        }
    }
}
