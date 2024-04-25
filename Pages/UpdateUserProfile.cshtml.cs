using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
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
        public ProfileSite UserProfileData { get; set; }
        public List<PageSite> PagesNewData { get; set; }
        public List<PageSite> PagesOldData { get; set; }
        public List<PageCategory> PageNewCategoriesData { get; set; }
        public List<PageCategory> PageOldCategoriesData { get; set; }
        [BindProperty]
        public string SelectedPagesJson { get; set; }

        [BindProperty]
        public string SelectedCategoriesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadUserProfile();
            await LoadExistingPagesData();
            await LoadRemainingPagesData();
            await LoadExistingCategoriesData();
            await LoadRemainingCategoriesData();
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid && UserProfileData.ProfileId < 0)
            {
                return Page();
            }

            PagesNewData = JsonConvert.DeserializeObject<List<PageSite>>(SelectedPagesJson);
            PageNewCategoriesData = JsonConvert.DeserializeObject<List<PageCategory>>(SelectedCategoriesJson);

            UserProfileDTO userProfileDTO = new UserProfileDTO
            {
                Profile = UserProfileData,
                Pages = PagesNewData,
                PageCategories = PageNewCategoriesData,
            };

            var client = _clientFactory.CreateClient();
            var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateUserProfile";
            var jsonContent = JsonConvert.SerializeObject(userProfileDTO);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            response = await client.PutAsync(apiUrl, httpContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Profile Updated Successfully";
                return RedirectToPage("/UserProfile");
            }
            else
            {
                TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
                return Page();
            }
        }

        private async Task LoadUserProfile()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserProfile");
            UserProfileData = JsonConvert.DeserializeObject<ProfileSite>(response);
        }
        private async Task LoadExistingPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserPages");
            PagesOldData = JsonConvert.DeserializeObject<List<PageSite>>(response);
        }
        private async Task LoadRemainingPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetNewUserPages");
            PagesNewData = JsonConvert.DeserializeObject<List<PageSite>>(response);
        }
        private async Task LoadExistingCategoriesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserCategories");
            PageOldCategoriesData = JsonConvert.DeserializeObject<List<PageCategory>>(response);
        }
        private async Task LoadRemainingCategoriesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetNewUserCategories");
            PageNewCategoriesData = JsonConvert.DeserializeObject<List<PageCategory>>(response);
        }
    }
}
