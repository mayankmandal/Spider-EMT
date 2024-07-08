using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Text;

namespace Spider_EMT.Pages
{
    public class UpdateUserProfileModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ProfileUserAPIVM _profileUserData { get; set; }


        public UpdateUserProfileModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public ProfileUserVM ProfileUsersData { get; set; }
        public List<ProfileSiteVM>? ProfilesData { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                await LoadAllProfilesData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        private async Task LoadAllProfilesData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            ProfilesData = JsonConvert.DeserializeObject<List<ProfileSiteVM>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            if(ProfileUsersData.UserId != null)
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetSettingsData");
                _profileUserData = JsonConvert.DeserializeObject<ProfileUserAPIVM>(response);

                if (_profileUserData.Username == ProfileUsersData.Username)
                {
                    ModelState.Remove("ProfileUsersData.Username");
                }

                if (_profileUserData.Email == ProfileUsersData.Email)
                {
                    ModelState.Remove("ProfileUsersData.Email");
                }

                if (_profileUserData.IdNumber == ProfileUsersData.IdNumber)
                {
                    ModelState.Remove("ProfileUsersData.IdNumber");
                }

                if (_profileUserData.MobileNo == ProfileUsersData.MobileNo)
                {
                    ModelState.Remove("ProfileUsersData.MobileNo");
                }
            }

            if (ProfileUsersData.PhotoFile == null)
            {
                ModelState.Remove("ProfileUsersData.PhotoFile");
            }

            if (ProfileUsersData.Password == null || ProfileUsersData.ReTypePassword == null)
            {
                ModelState.Remove("ProfileUsersData.Password");
                ModelState.Remove("ProfileUsersData.ReTypePassword");
            }

            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                TempData["error"] = "Model State Validation Failed.";
                return Page();
            }
            try
            {
                string uniqueFileName = null;
                string filePath = null;
                string uploadFolder = null;

                if (ProfileUsersData.PhotoFile != null)
                {
                    uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"]);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileUsersData.PhotoFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // FileStream is properly disposed of after use
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileUsersData.PhotoFile.CopyToAsync(fileStream);
                    }
                }

                ProfileUserAPIVM profileUserAPIVM = new ProfileUserAPIVM
                {
                    UserId = ProfileUsersData.UserId,
                    IdNumber = ProfileUsersData.IdNumber,
                    Email = ProfileUsersData.Email,
                    MobileNo = ProfileUsersData.MobileNo,
                    FullName = ProfileUsersData.FullName,
                    Password = "",
                    Username = ProfileUsersData.Username,
                    Userimgpath = ProfileUsersData.PhotoFile != null ? uniqueFileName : "",
                    ProfileSiteData = ProfileUsersData.ProfileSiteData,
                    IsActive = ProfileUsersData.IsActive,
                    IsActiveDirectoryUser = ProfileUsersData.IsActiveDirectoryUser,
                    ChangePassword = ProfileUsersData.ChangePassword,
                    CreateUserId = ProfileUsersData.CreateUserId,
                    UpdateUserId = ProfileUsersData.UpdateUserId
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateUserProfile";
                var jsonContent = JsonConvert.SerializeObject(profileUserAPIVM);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = $"{ProfileUsersData.FullName} - Profile Updated Successfully";
                    return RedirectToPage();
                }
                else
                {
                    await LoadAllProfilesData();
                    TempData["error"] = $"{ProfileUsersData.FullName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
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
            TempData["error"] = $"{ProfileUsersData.FullName} - " + errorMessage + ". Error details: " + ex.Message;
            return Page();
        }
    }
}
