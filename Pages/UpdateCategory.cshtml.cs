using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Text;

namespace Spider_EMT.Pages
{
    public class UpdateCategoryModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UpdateCategoryModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public List<PageSiteVM>? AllPageSites { get; set; }
        [BindProperty]
        public List<PageCategoryVM>? AllCategories { get; set; }
        [BindProperty]
        public PageCategoryVM? SelectedPageCategory { get; set; }
        [BindProperty]
        public string? SelectedPagesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadAllCategoriesData();
                await LoadAllPagesData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        private async Task LoadAllCategoriesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllCategories");
            AllCategories = JsonConvert.DeserializeObject<List<PageCategoryVM>>(response);
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
                await LoadAllCategoriesData();
                await LoadAllPagesData();
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSiteVM>>(SelectedPagesJson);
                PageCategoryVM selectedProfileData = new PageCategoryVM
                {
                    PageCatId = Int32.Parse(SelectedPageCategory.CategoryName),
                    CategoryName = Constants.MagicString
                };

                CategoryPagesAccessDTO categoryPagesAccessDTO = new CategoryPagesAccessDTO
                {
                    PageCategoryData = selectedProfileData,
                    PagesList = selectedPages
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateCategory";
                var jsonContent = JsonConvert.SerializeObject(categoryPagesAccessDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $"Category Updated Successfully" });
                }
                else
                {
                    await LoadAllCategoriesData();
                    await LoadAllPagesData();
                    return new JsonResult(new { success = true, message = $"Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = errorMessage + ". Error details: " + ex.Message });
        }
    }
}
