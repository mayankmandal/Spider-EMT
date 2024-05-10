using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class CreateCategoryModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public CreateCategoryModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        [BindProperty]
        public List<PageSite> AllPageSites { get; set; }
        [BindProperty]
        public PageCategory SelectedPageCategory { get; set; }
        [BindProperty]
        public string SelectedPagesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadAllPagesData();
            return Page();
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
                await LoadAllPagesData();
                return Page(); // Return to the same page if validation fails
            }
            try
            {
                // Deserialize the Json string into a list of PageSite objects
                var selectedPages = JsonConvert.DeserializeObject<List<PageSite>>(SelectedPagesJson);
                PageCategory selectedProfileData = new PageCategory
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
                    TempData["success"] = "New Category Created Successfully";
                    return RedirectToPage("/Index");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode + response.RequestMessage + response.ReasonPhrase;
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
