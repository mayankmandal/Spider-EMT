using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class UpdateUserAccessControlModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UpdateUserAccessControlModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public List<ProfileSite> AllProfilesData { get; set; }
        [BindProperty]
        public List<PageSite> AllPageSites { get; set; }
        [BindProperty]
        public string SelectedProfileId { get; set; }
        [BindProperty]
        public string SelectedProfileName { get; set; }
        [BindProperty]
        public string SelectedPagesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadAllProfilesData();
            await LoadAllPagesData();
            return Page();
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
                    TempData["success"] = "Profile Created Successfully";
                    return RedirectToPage("/Index");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
                    await LoadAllProfilesData();
                    await LoadAllPagesData();
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
