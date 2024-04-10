using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Pages
{
    public class UserProfileModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UserProfileModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
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
            //await LoadUserProfile();
            await LoadPagesData();
            //await LoadCategoriesData();
            return Page();
        }
        private async Task LoadPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            PagesData = JsonConvert.DeserializeObject<List<PageSite>>(response);
        }
    }
}
