using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Models;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages.Account
{
    public class UserRoleAssignmentModel : PageModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ProfileUserAPIVM CurrentUserDetailsData { get; set; }
        public UserRoleAssignmentModel(ICurrentUserService currentUserService, IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _currentUserService = currentUserService;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public UserRoleAssignmentVM UserRoleAssignmentVMData { get; set; }
        public List<ProfileSiteVM>? ProfilesData { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _currentUserService.UserManager.GetUserAsync(base.User);
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
                // Validate the selected profile ID
                /*var profileId = UserRoleAssignmentVMData.ProfileSiteData.ProfileId;
                if (string.IsNullOrEmpty(profileId))
                {
                    ModelState.AddModelError("UserRoleAssignmentVMData.ProfileSiteData.ProfileName", "Please select a valid profile.");
                    ProfilesData = await FetchProfilesAsync();
                    return Page();
                }

                // Logic to assign the profile to the user (could involve calling an API, updating the database, etc.)
                var result = await AssignProfileToUserAsync(profileId);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Error assigning profile to user.");
                    ProfilesData = await FetchProfilesAsync();
                    return Page();
                }*/

                TempData["success"] = "Profile assigned successfully.";
                return RedirectToPage("/Dashboard");
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
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserDetails");
            CurrentUserDetailsData = JsonConvert.DeserializeObject<ProfileUserAPIVM>(response);
        }

    }
}
