using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class ProfileCategoryAssignModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public ProfileCategoryAssignModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public List<ProfileSite> AllProfilesData { get; set; }
        public List<PageCategory> AllCategoriesData { get; set; }
        [BindProperty]
        public int SelectedProfileId { get; set; }
        [BindProperty]
        public string SelectedProfileName { get; set; }
        [BindProperty]
        public string SelectedCategoriesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadAllProfilesData();
            await LoadAllCategoriesData();
            return Page();
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            AllProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(response);
        }
        private async Task LoadAllCategoriesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllCategories");
            AllCategoriesData = JsonConvert.DeserializeObject<List<PageCategory>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData();
                await LoadAllCategoriesData();
                return Page(); // Return to the same page if validation fails
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
                var jsonContent = JsonConvert.SerializeObject(profileCategoryAccessDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Relation between Profile and Categories Created Successfully";
                    return RedirectToPage("/ProfileCategoryAssign");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
                    await LoadAllProfilesData();
                    await LoadAllCategoriesData();
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
