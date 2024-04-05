using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Text;
using Page = Spider_EMT.Models.Page;

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
        public Profile userProfile { get; set; }
        [BindProperty]
        public IEnumerable<Page> Pages { get; set; }
        [BindProperty]
        public IEnumerable<PageCategory> PageCategories { get; set; }
        public async Task OnGet()
        {
            await LoadPagesData();
            await LoadCategoryData();
        }
        private async Task LoadPagesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages");
            Pages = JsonConvert.DeserializeObject<IEnumerable<Page>>(response);
        }
        private async Task LoadCategoryData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPageCategories");
            PageCategories = JsonConvert.DeserializeObject<IEnumerable<PageCategory>>(response);
        }
        public async Task<IActionResult> OnPost() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrlforUserProfile = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateUserProfile";
                var jsonContentforUserProfile = JsonConvert.SerializeObject(userProfile);
                var httpContentforUserProfile = new StringContent(jsonContentforUserProfile, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessageforUserProfile;

                if(userProfile.ProfileId > 0)
                {
                    responseMessageforUserProfile = await client.PutAsync(apiUrlforUserProfile, httpContentforUserProfile);
                }
                else
                {
                    // API endpoint adding new data
                    apiUrlforUserProfile = $"{_configuration["ApiBaseUrl"]}/Navigation/AddUserProfile";
                    responseMessageforUserProfile = await client.PostAsync(apiUrlforUserProfile, httpContentforUserProfile);

                    /*var apiUrlforPages = $"{_configuration["ApiBaseUrl"]}/Navigation/GetAllPages";
                    var jsonContentforPages = JsonConvert.SerializeObject(pages);
                    var httpContentforPages = new StringContent(jsonContentforPages, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessageforPages;

                    if (pages.Count > 0)
                    {
                        responseMessageforPages = await client.PostAsync(apiUrlforPages, httpContentforPages);
                    }*/
                }

                if (responseMessageforUserProfile.IsSuccessStatusCode)
                {
                    if(userProfile.ProfileId > 0)
                    {
                        TempData["success"] = "Profile Updated Successfully";
                    }
                    else
                    {
                        TempData["success"] = "Profile Created Successfully";
                    }
                    // Redirect to Index Page
                    return RedirectToPage("/Index");
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + responseMessageforUserProfile.StatusCode;
                    // Redirect to the same page  if error occured
                    return Page();
                }

            }
            catch (Exception ex)
            {
                TempData["error"] = "Error Occured : " + ex.Message;
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
