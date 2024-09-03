using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace Spider_EMT.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public ForgotPasswordModel(ICurrentUserService currentUserService, IEmailService emailService, IConfiguration configuration)
        {
            _currentUserService = currentUserService;
            _emailService = emailService;
            _configuration = configuration;
        }
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid input. Please check your data and try again.";
                return Page();
            }

            var user = await _currentUserService.UserManager.FindByEmailAsync(Email);
            if (user == null || !(await _currentUserService.UserManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var confirmationToken = await _currentUserService.UserManager.GeneratePasswordResetTokenAsync(user);
            confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

            var callbackUrl = Url.Page(
                pageName: "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = confirmationToken },
                protocol: Request.Scheme
                );

            await _emailService.SendAsync(_configuration["EmailServiceSender"], user.Email, "Reset Password", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
