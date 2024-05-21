using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class CreateUserAccessControlModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public CreateUserAccessControlModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public List<ProfileSite>? AllProfilesData { get; set; }
        public List<PageSite>? AllPageSites { get; set; }
        [BindProperty]
        public string? SelectedProfileId { get; set; }
        [BindProperty]
        public string? SelectedProfileName { get; set; }
        [BindProperty]
        public string? SelectedPagesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {

            try
            {
                await LoadAllProfilesData();
                await LoadAllPagesData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading page data.");
            }
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            AllProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(response);
        }
        private async Task LoadAllPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            AllPageSites = JsonConvert.DeserializeObject<List<PageSite>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData();
                await LoadAllPagesData();
                return Page(); // Return to the same page if validation fails
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSite>>(SelectedPagesJson);
                ProfileSite selectedProfileData = new ProfileSite
                {
                    ProfileId = JsonConvert.DeserializeObject<int>(SelectedProfileId),
                    ProfileName = SelectedProfileName
                };

                ProfilePagesAccessDTO profilePageDTO = new ProfilePagesAccessDTO
                {
                    ProfileData = selectedProfileData,
                    PagesList = selectedPages
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/CreateUserAccess";
                var jsonContent = JsonConvert.SerializeObject(profilePageDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "User Access Control Created Successfully";
                    return RedirectToPage("/CreateUserAccessControl");
                }
                else
                {
                    TempData["error"] = $"Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
                    await LoadAllProfilesData();
                    await LoadAllPagesData();
                    return Page();
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
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + " Error details: " + ex.Message;
            return RedirectToPage("/Error");
        }
    }
}
