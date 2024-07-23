using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Configuration.Service;
using Spider_EMT.Data.Account;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace Spider_EMT.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public RegisterModel(IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
        }
        [BindProperty]
        public RegisterViewModel RegisterVM { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            // Create User
            var user = new ApplicationUser
            {
                Email = RegisterVM.Email,
                UserName = RegisterVM.Email,
            };

            var claimDepartment = new Claim("Department", RegisterVM.Department);
            var claimPosition = new Claim("Position", RegisterVM.Position);

            var result = await _userManager.CreateAsync(user, RegisterVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, claimDepartment);
                await _userManager.AddClaimAsync(user, claimPosition);

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                await _emailService.SendAsync(_configuration["EmailServiceSender"], user.Email, "Please confirm your email", $"Please click on this link to confirm your email address: {confirmationLink}");
                return RedirectToPage("/Account/Login");

            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return Page();
            }
        }
    }
}
