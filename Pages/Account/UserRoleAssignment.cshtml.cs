using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Microsoft.AspNetCore.Authorization;
using Spider_EMT.Utility;
using System.Net.Http.Headers;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
    public class UserRoleAssignmentModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private ProfileUserAPIVM CurrentUserDetailsData { get; set; }
        public UserRoleAssignmentModel(ICurrentUserService currentUserService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _currentUserService = currentUserService;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public UserRoleAssignmentVM UserRoleAssignmentVMData { get; set; }
        public List<ProfileSiteVM>? ProfilesData { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _currentUserService.RefreshCurrentUserAsync();
                var user = await _currentUserService.GetCurrentUserAsync();

                if (user == null ||
                    user.EmailConfirmed != true ||
                    user.TwoFactorEnabled != true ||
                    user.UserVerificationSetupEnabled != true)
                {
                    TempData["error"] = $"Invalid login attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }
                if (user.RoleAssignmentEnabled == true)
                {
                    TempData["success"] = $"{user.FullName} logged in successfully.";
                    return RedirectToPage("/Dashboard");
                }

                await LoadCurrentProfileUserData();

                UserRoleAssignmentVMData = new UserRoleAssignmentVM
                {
                    Email = CurrentUserDetailsData.Email,
                    FullName = CurrentUserDetailsData.FullName,
                    ProfileSiteData = new ProfileSiteVM
                    {
                        ProfileName = CurrentUserDetailsData.ProfileSiteData.ProfileName,
                        ProfileId = CurrentUserDetailsData.ProfileSiteData.ProfileId
                    }
                };
                return Page();

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
                // Re-fetch profiles data for the dropdown in case of an invalid model state
                await LoadCurrentProfileUserData();
                return Page();
            }

            try
            {
                await _currentUserService.RefreshCurrentUserAsync();
                var user = await _currentUserService.GetCurrentUserAsync();

                if (user == null ||
                user.EmailConfirmed != true ||
                user.TwoFactorEnabled != true ||
                user.UserVerificationSetupEnabled != true)
                {
                    TempData["error"] = $"Invalid login attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }

                if (user.RoleAssignmentEnabled == true)
                {
                    _currentUserService.RefreshCurrentUserAsync();
                    TempData["success"] = $"{user.FullName} logged in successfully.";
                    return RedirectToPage("/Dashboard");
                }
                TempData["error"] = $"Proper Role is not yet Assigned. Contact the administrator.";
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while assigning profile.");
            }
        }
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = $"{CurrentUserDetailsData.FullName} - " + errorMessage + ". Error details: " + ex.Message;
            return Page();
        }

        private async Task LoadCurrentProfileUserData()
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserDetails");
            CurrentUserDetailsData = JsonConvert.DeserializeObject<ProfileUserAPIVM>(response);
        }

    }
}
