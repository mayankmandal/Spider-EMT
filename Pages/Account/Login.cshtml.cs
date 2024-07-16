using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Models.ViewModels;
using System.Security.Claims;

namespace Spider_EMT.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public CredentialVM? CredentialData { get; set; }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            
            // DB Call for verification for temporary checking here itself

            // Verify the Credentials
            if(CredentialData.Username == "admin" && CredentialData.Password == "password")
            {
                // Creating the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email,"admin@example.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("EmployeeJoiningDate","2024-04-02")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = CredentialData.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);
                return RedirectToPage("/Dashboard");
            }
            return Page();
        }
    }
}
