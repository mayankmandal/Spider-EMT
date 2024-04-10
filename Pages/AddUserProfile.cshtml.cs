using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class AddUserProfileModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public AddUserProfileModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public ProfileSite userProfileData { get; set; }
        public List<PageSite> PagesData { get; set; }
        public List<PageCategory> PageCategoriesData { get; set; }
        [BindProperty]
        public string SelectedPagesJson { get; set; }

        [BindProperty]
        public string SelectedCategoriesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadPagesData();
            return Page();
        }
        private async Task LoadPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            PagesData = JsonConvert.DeserializeObject<List<PageSite>>(response);
        }
        public async Task<IActionResult> OnPost() 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                PagesData = JsonConvert.DeserializeObject<List<PageSite>>(SelectedPagesJson);
                PageCategoriesData = JsonConvert.DeserializeObject<List<PageCategory>>(SelectedCategoriesJson);

                UserProfileDTO userProfileDTO = new UserProfileDTO
                {
                    Profile = userProfileData,
                    Pages = PagesData,
                    PageCategories = PageCategoriesData,
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/AddUserProfile";
                var jsonContent = JsonConvert.SerializeObject(userProfileDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Profile Created Successfully";
                    return RedirectToPage("/AddUserProfile");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
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
