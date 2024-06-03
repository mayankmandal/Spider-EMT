using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using System.Text;
using static Spider_EMT.Utility.Constants;

namespace Spider_EMT.Pages
{
    public class UpdateUserProfileModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UpdateUserProfileModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public ProfileUser? ProfileUsersData { get; set; }
        public List<ProfileSite>? ProfilesData { get; set; }
        [BindProperty]
        public List<string>? UserStatusLst { get; set; }
        public List<CheckBoxOption> Checkboxes = UserStatusDescription.StatusOptions.Select(option => new CheckBoxOption
        {
            IsChecked = false,
            Text = option.Text,
            Value = option.Value
        }).ToList();
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadAllProfilesData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            ProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(response);
        }
        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
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
                    return new JsonResult(new { success = true, message = $"{ProfileUsersData.FullName} - Profile Updated Successfully" });
                }
                else
                {
                    await LoadAllProfilesData();
                    return new JsonResult(new { success = true, message = $"{ProfileUsersData.FullName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
                }
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
        private JsonResult HandleError(Exception ex, string errorMessage)
        {
            return new JsonResult(new { success = false, message = $"{ProfileUsersData.FullName} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
