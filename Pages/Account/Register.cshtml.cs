using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Diagnostics;

namespace Spider_EMT.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        // private readonly RecaptchaEnterpriseServiceClient _recaptchaEnterpriseServiceClient;
        // private readonly GoogleReCaptchaSettings _googleReCaptchaSettings;

        public RegisterModel(IConfiguration configuration, ICurrentUserService currentUserService, IEmailService emailService) //RecaptchaEnterpriseServiceClient recaptchaClient, IOptions<GoogleReCaptchaSettings> googleReCaptchaSettings)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
            _emailService = emailService;
            // _recaptchaEnterpriseServiceClient = recaptchaClient;
            // _googleReCaptchaSettings = googleReCaptchaSettings.Value;
        }
        [BindProperty]
        public RegisterViewModel RegisterVM { get; set; }
        // [BindProperty]
        // public string ReCaptchaToken { get; set; }

        public void OnGet()
        {
            // ViewData["ReCaptchaSiteKey"] = _googleReCaptchaSettings.SiteKey;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            // Verify ReCaptcha
            /*var projectName = _configuration["GoogleCloud_GoogleReCaptcha_ProjectId"];
            var assessment = await _recaptchaEnterpriseServiceClient.CreateAssessmentAsync(new CreateAssessmentRequest
            {
                Assessment = new Assessment
                {
                    Event = new Event
                    {
                        SiteKey = _googleReCaptchaSettings.SiteKey,
                        Token = ReCaptchaToken
                    }
                },
                Parent = projectName
            });

            if (!assessment.TokenProperties.Valid)
            {
                ModelState.AddModelError("Register", "ReCaptcha validation failed.");
                return Page();
            }*/
            
            // Create User
            var user = new ApplicationUser
            {
                Email = RegisterVM.Email,
                FullName = RegisterVM.FullName,
                UserName = RegisterVM.Email
            };

            var result = await _currentUserService.UserManager.CreateAsync(user, RegisterVM.Password);
            if (result.Succeeded)
            {
                var confirmationToken = await _currentUserService.UserManager.GenerateEmailConfirmationTokenAsync(user);
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
