using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Text;

namespace Spider_EMT.Pages
{
    public class ReadUserProfileModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public ReadUserProfileModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public CurrentUser CurrentUserData { get; set; }
        public ProfileUser CurrentUserDetailsData { get; set; }
        public List<PageSite> CurrentPageSites { get; set; }
        private List<CategoriesSetDTO> CurrentCategoriesSetDTOs { get; set; }
        public List<string> StatusCheckboxes { get; set; }
        public List<CategoryDisplayViewModel> StructureData { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadCurrentUserData();
            await LoadCurrentProfileUserData();
            await LoadCurrentPageSites();
            await LoadCurrentCategoriesSetDTOs();

            StatusCheckboxes = Constants.UserStatusDescription.GetStatusTextsFromCsv(CurrentUserDetailsData.UserStatus.ToString());
            return Page();
        }
        private async Task LoadCurrentUserData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUser");
            CurrentUserData = JsonConvert.DeserializeObject<CurrentUser>(response);
        }
        private async Task LoadCurrentProfileUserData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserDetails");
            CurrentUserDetailsData = JsonConvert.DeserializeObject<ProfileUser>(response);
        }
        private async Task LoadCurrentPageSites()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserPages");
            CurrentPageSites = JsonConvert.DeserializeObject<List<PageSite>>(response);
            CurrentPageSites = CurrentPageSites.OrderBy(page => page.PageDescription).ToList();
        }
        private async Task LoadCurrentCategoriesSetDTOs()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetCurrentUserCategories");
            CurrentCategoriesSetDTOs = JsonConvert.DeserializeObject<List<CategoriesSetDTO>>(response);
            ProcessCategories();
        }
        private void ProcessCategories()
        {
            StructureData = new List<CategoryDisplayViewModel>();

            // Group pages by category name
            var groupedCategories = CurrentCategoriesSetDTOs.GroupBy(cat => cat.CatagoryName);

            foreach(var categoryGroup in groupedCategories)
            {
                var category = new CategoryDisplayViewModel
                {
                    CategoryName = categoryGroup.Key,
                    Pages = categoryGroup.Select(page => new PageDisplayViewModel
                    {
                         PageDescription = page.PageDescription,
                         PageUrl = page.PageUrl,
                    }).ToList()
                };
                StructureData.Add(category);
            }

            // Sort the pages within each category
            foreach (var category in StructureData)
            {
                category.Pages = category.Pages.OrderBy(page => page.PageDescription).ToList();
            }

            // Sort the categories
            StructureData = StructureData.OrderBy(cat => cat.CategoryName).ToList();
        }
    }
}
