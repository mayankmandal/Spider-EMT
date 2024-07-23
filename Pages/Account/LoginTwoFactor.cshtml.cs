using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Data.Account;
using System.Drawing.Printing;

namespace Spider_EMT.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        [BindProperty]
        public EmailMFA EmailMFAData { get; set; }
        public LoginTwoFactorModel(IConfiguration configuration,UserManager<ApplicationUser> userManager, IEmailService emailService, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            EmailMFAData = new EmailMFA();

        }
        public async Task OnGetAsync(string email,bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);

            EmailMFAData.SecurityCode = string.Empty;
            EmailMFAData.RememberMe = rememberMe;

            // Generate the Code
            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            // Send to the User
            await _emailService.SendAsync(_configuration["EmailServiceSender"], email, "Spider_EMT Login OTP",$"Please use this code as OTP: {securityCode}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result =await _signInManager.TwoFactorSignInAsync("Email", EmailMFAData.SecurityCode, EmailMFAData.RememberMe,false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Dashboard");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LoginTwoFactor", "You are locked out!");
                }
                else
                {
                    ModelState.AddModelError("LoginTwoFactor", "Failed to Login.");
                }
                return Page();
            }

        }
    }
}
