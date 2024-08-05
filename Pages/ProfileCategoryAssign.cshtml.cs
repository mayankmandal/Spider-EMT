using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Net.Http.Headers;
using System.Text;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
    public class ProfileCategoryAssignModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public ProfileCategoryAssignModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public ProfileSiteVM? profileSite { get; set; }
        public List<ProfileSiteVM>? AllProfilesData { get; set; }
        public List<PageCategoryVM>? AllCategoriesData { get; set; }
        [BindProperty]
        public int SelectedProfileId { get; set; }
        [BindProperty]
        public string? SelectedProfileName { get; set; }
        [BindProperty]
        public string? SelectedCategoriesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadAllProfilesData();
                await LoadAllCategoriesData();
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            AllProfilesData = JsonConvert.DeserializeObject<List<ProfileSiteVM>>(response);
        }
        private async Task LoadAllCategoriesData()
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllCategories");
            AllCategoriesData = JsonConvert.DeserializeObject<List<PageCategoryVM>>(response);
        }
        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData();
                await LoadAllCategoriesData();
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedCategories = JsonConvert.DeserializeObject<List<PageCategory>>(SelectedCategoriesJson);
                ProfileSite selectedProfileData = new ProfileSite
                {
                    ProfileId = SelectedProfileId,
                    ProfileName = SelectedProfileName
                };

                ProfileCategoryAccessDTO profileCategoryAccessDTO = new ProfileCategoryAccessDTO
                {
                    ProfileSiteData = selectedProfileData,
                    PageCategories = selectedCategories
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/AssignProfileCategories";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
                var jsonContent = JsonConvert.SerializeObject(profileCategoryAccessDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $"{SelectedProfileName} - Relation between Profile and Categories Created Successfully" });
                }
                else
                {
                    await LoadAllProfilesData();
                    await LoadAllCategoriesData();
                    return new JsonResult(new { success = true, message = $"{SelectedProfileName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $"{SelectedProfileName} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
