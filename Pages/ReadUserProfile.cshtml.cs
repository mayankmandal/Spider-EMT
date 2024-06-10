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
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public ReadUserProfileModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public CurrentUser? CurrentUserData { get; set; }
        public ProfileUserVM CurrentUserDetailsData { get; set; }
        public List<PageSite>? CurrentPageSites { get; set; }
        public List<string>? StatusCheckboxes { get; set; }
        public List<CategoryDisplayViewModel>? StructureData { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadCurrentUserData();
                await LoadCurrentProfileUserData();
                await LoadCurrentPageSites();
                await LoadCurrentCategoriesSetDTOs();

                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
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
            CurrentUserDetailsData = JsonConvert.DeserializeObject<ProfileUserVM>(response);
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
            StructureData = JsonConvert.DeserializeObject<List<CategoryDisplayViewModel>>(response);
        }
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + " Error details: " + ex.Message;
            return RedirectToPage("/Index");
        }
    }
}
