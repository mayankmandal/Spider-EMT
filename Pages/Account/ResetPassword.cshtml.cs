using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Spider_EMT.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        public ResetPasswordModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Code { get; set; }
        }
        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _currentUserService.UserManager.FindByEmailAsync(Input.Email);
            if(user == null)
            {
                return RedirectToPage("./ResetPasswordConfirmation"); // Not to reveal that  user does not exist
            }
            // Check if NormalizedUserName and NormalizedEmail are not the same
            if (user.NormalizedUserName != user.NormalizedEmail)
            {
                user.NormalizedUserName = user.NormalizedEmail;
                await _currentUserService.UserManager.UpdateNormalizedUserNameAsync(user);
            }
            var result = await _currentUserService.UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
