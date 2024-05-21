using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Text;
using Spider_EMT.Utility;

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
        public List<PageSite>? AllPageSites { get; set; }
        [BindProperty]
        public List<PageCategory>? AllCategories { get; set; }
        [BindProperty]
        public PageCategory? SelectedPageCategory { get; set; }
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
            AllCategories = JsonConvert.DeserializeObject<List<PageCategory>>(response);
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
                await LoadAllCategoriesData();
                await LoadAllPagesData();
                return Page(); // Return to the same page if validation fails
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSite>>(SelectedPagesJson);
                PageCategory selectedProfileData = new PageCategory
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
                    TempData["success"] = "User Access Control Updated Successfully";
                    return RedirectToPage("/UpdateCategory");
                }
                else
                {
                    TempData["error"] = $"Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
                    await LoadAllCategoriesData();
                    await LoadAllPagesData();
                    return Page();
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
        private IActionResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + " Error details: " + ex.Message;
            return RedirectToPage("/Error");
        }
    }
}
