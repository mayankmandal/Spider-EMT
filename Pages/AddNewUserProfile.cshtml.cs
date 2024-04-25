using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class AddNewUserProfileModel : PageModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public AddNewUserProfileModel(INavigationRepository navigationRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _navigationRepository = navigationRepository;
            _configuration = configuration;
            _clientFactory = httpClientFactory;
        }
        public List<ProfileSite> ProfilesData { get; set; }
        public List<ProfileUser> ProfileUsersData { get; set; }
        [BindProperty]
        public string SelectedProfileUsersJson { get; set; }
        [BindProperty]
        public string SelectedProfilesJson { get; set; }
        public async Task<IActionResult> OnGet()
        {
            await LoadAllProfilesData();
            await LoadAllProfileUsersData();
            return Page();
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            ProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(response);
        }
        private async Task LoadAllProfileUsersData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfileUsers");
            ProfileUsersData = JsonConvert.DeserializeObject<List<ProfileUser>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ProfilesData = JsonConvert.DeserializeObject<List<ProfileSite>>(SelectedProfilesJson);
                ProfileUsersData = JsonConvert.DeserializeObject<List<ProfileUser>>(SelectedProfileUsersJson);

                UserToProfileDTO userToProfileDTO = new UserToProfileDTO
                {
                    Profiles = ProfilesData,
                    ProfileUsers = ProfileUsersData
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/AddNewUserProfile";
                var jsonContent = JsonConvert.SerializeObject(userToProfileDTO);
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
