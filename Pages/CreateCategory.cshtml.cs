using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Models;
using System.Text;

namespace Spider_EMT.Pages
{
    public class CreateCategoryModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public CreateCategoryModel(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public List<PageSite>? AllPageSites { get; set; }
        [BindProperty]
        public PageCategory? SelectedPageCategory { get; set; }
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
            AllPageSites = JsonConvert.DeserializeObject<List<PageSite>>(response);
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
                PageCategoryVM selectedProfileData = new PageCategoryVM
                {
                    CategoryName = SelectedPageCategory.CategoryName
                };

                CategoryPagesAccessDTO categoryPagesAccessDTO = new CategoryPagesAccessDTO
                {
                    PageCategoryData = selectedProfileData,
                    PagesList = selectedPages
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/CreateNewCategory";
                var jsonContent = JsonConvert.SerializeObject(categoryPagesAccessDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $"{SelectedPageCategory.CategoryName} - New Category Created Successfully" });
                }
                else
                {
                    await LoadAllPagesData();
                    return new JsonResult(new { success = true, message = $"{SelectedPageCategory.CategoryName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $"{SelectedPageCategory.CategoryName} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
