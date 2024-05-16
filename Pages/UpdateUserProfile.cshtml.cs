using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Text;

namespace Spider_EMT.Pages
{
    public class UpdateUserProfileModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UpdateUserProfileModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public ProfileUser ProfileUsersData { get; set; }
        public List<ProfileSite> ProfilesData { get; set; }
        [BindProperty]
        public List<string> UserStatusLst { get; set; }
        public List<CheckBoxOption> Checkboxes = new List<CheckBoxOption>
        {
            new CheckBoxOption()
            {
                IsChecked = false,
                Text = Constants.UserStatusDescription.IsActive.Text,
                Value = Constants.UserStatusDescription.IsActive.Value
            },
            new CheckBoxOption()
            {
                IsChecked = false,
                Text = Constants.UserStatusDescription.IsActiveDirectoryUser.Text,
                Value = Constants.UserStatusDescription.IsActiveDirectoryUser.Value
            },
            new CheckBoxOption()
            {
                IsChecked = false,
                Text = Constants.UserStatusDescription.ChangePassword.Text,
                Value = Constants.UserStatusDescription.ChangePassword.Value
            }
        };
        public async Task<IActionResult> OnGet()
        {
            await LoadAllProfilesData();
            return Page();
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            ProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                return Page(); // Return to the same page if validation fails
            }
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateUserProfile";
                var jsonContent = JsonConvert.SerializeObject(ProfileUsersData);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Profile Updated Successfully";
                    return RedirectToPage("/CreateUserAccessControl");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
                    await LoadAllProfilesData();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error occured : " + ex.Message;
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
