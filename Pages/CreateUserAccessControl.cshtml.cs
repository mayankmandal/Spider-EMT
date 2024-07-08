using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
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
        public List<PageSiteVM>? AllPageSites { get; set; }
        [BindProperty]
        public ProfileSiteVM ProfileSiteData { get; set; }
        [BindProperty]
        public string? SelectedPagesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadAllPagesData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading page data.");
            }
        }
        private async Task LoadAllPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            AllPageSites = JsonConvert.DeserializeObject<List<PageSiteVM>>(response);
        }
        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllPagesData();
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSiteVM>>(SelectedPagesJson);
                ProfileSiteVM selectedProfileData = new ProfileSiteVM
                {
                    ProfileName = ProfileSiteData.ProfileName
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
                    return new JsonResult(new { success = true, message = $"{ProfileSiteData.ProfileName} - Access Control Created Successfully" });
                }
                else
                {
                    await LoadAllPagesData();
                    return new JsonResult(new { success = true, message = $"{ProfileSiteData.ProfileName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $"{ProfileSiteData.ProfileName} - " + errorMessage + ". Error details: " + ex.Message });

        }
    }
}
