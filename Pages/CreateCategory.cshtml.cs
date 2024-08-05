using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
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
        public List<PageSiteVM>? AllPageSites { get; set; }
        [BindProperty]
        public PageCategoryVM? SelectedPageCategory { get; set; }
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            AllPageSites = JsonConvert.DeserializeObject<List<PageSiteVM>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await LoadAllPagesData();
                TempData["error"] = "Model State Validation Failed.";
                return Page();
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSiteVM>>(SelectedPagesJson);
                PageCategory selectedProfileData = new PageCategory
                {
                    CategoryName = SelectedPageCategory.CategoryName
                };

                CategoryPagesAccessDTO categoryPagesAccessDTO = new CategoryPagesAccessDTO
                {
                    PageCategoryData = selectedProfileData,
                    PagesList = selectedPages
                };

                var client = _clientFactory.CreateClient("WebAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/CreateNewCategory";
                var jsonContent = JsonConvert.SerializeObject(categoryPagesAccessDTO);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["success"] = $"{SelectedPageCategory.CategoryName} - New Category Created Successfully" ;
                    return RedirectToPage();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await LoadAllPagesData();
                    TempData["error"] = $"{SelectedPageCategory.CategoryName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase} - Please login again";
                    return Page();
                }
                else
                {
                    await LoadAllPagesData();
                    TempData["error"] = $"{SelectedPageCategory.CategoryName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
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
            TempData["error"] = $"{SelectedPageCategory.CategoryName} - " + errorMessage + ". Error details: " + ex.Message;
            return Page();
        }
    }
}
