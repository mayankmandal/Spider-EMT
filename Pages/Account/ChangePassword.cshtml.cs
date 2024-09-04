using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
    public class ChangePasswordModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordModel(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        [BindProperty]
        public ChangePasswordVM ChangePasswordVMData { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null || !user.EmailConfirmed || !user.TwoFactorEnabled)
                {
                    TempData["error"] = $"Invalid attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }
                if (user != null)
                {
                    ChangePasswordVMData = new ChangePasswordVM()
                    {
                        OldPassword = string.Empty,
                        NewPassword = string.Empty,
                        ConfirmPassword = string.Empty
                    };
                    
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Account/AccessDenied");
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Model State Validation Failed.";
                return Page();
            }

            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null || !user.EmailConfirmed || !user.TwoFactorEnabled)
                {
                    TempData["error"] = $"Invalid attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }
                if (ChangePasswordVMData.NewPassword == null )
                {
                    ModelState.AddModelError("ChangePasswordVMData.NewPassword", "New Password is empty");
                }
                if(ChangePasswordVMData.ConfirmPassword == null)
                {
                    ModelState.AddModelError("ChangePasswordVMData.ConfirmPassword", "Confirm Password is empty");
                }

                var changePasswordResult = await _currentUserService.UserManager.ChangePasswordAsync(user, ChangePasswordVMData.OldPassword,ChangePasswordVMData.NewPassword);
                if(!changePasswordResult.Succeeded)
                {
                    foreach(var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                await _currentUserService.SignInManager.RefreshSignInAsync(user);
                TempData["success"] = $"{user.FullName} - Password has been changed successfully.";
                return RedirectToPage("/Dashboard");
            }
            catch (HttpRequestException ex)
            {
                return HandleError(ex, "Error occurred during HTTP request.");
            }
            catch (JsonException ex)
            {
                return HandleError(ex, "Error occurred while parsing JSON.");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "An unexpected error occurred.");
            }
        }
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + ". Error details: " + ex.Message;
            return Page();
        }
    }
}
