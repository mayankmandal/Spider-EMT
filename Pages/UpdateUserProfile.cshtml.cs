using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
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
        public string UserProfilePathUrl = string.Empty;
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
            bool isProfilePhotoReUpload = true;

            if (ProfileUsersData.UserId != null)
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/FetchUserRecord";
                // Create a request with the user ID in the body
                UserIdRequest requestBody = new UserIdRequest { UserId = ProfileUsersData.UserId };
                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _profileUserData = JsonConvert.DeserializeObject<ProfileUserAPIVM>(responseContent);
                }
                else
                {
                    await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                    ModelState.AddModelError("ProfileUsersData.UserId", $"Error fetching user record for {ProfileUsersData.UserId}. Please ensure the UserId is correct.");
                    TempData["error"] = $"Model State Validation Failed. Response status: {response.StatusCode} - {response.ReasonPhrase}";
                    return Page();
                }

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
                isProfilePhotoReUpload = false;
            }

            if (ProfileUsersData.Password == null || ProfileUsersData.ReTypePassword == null)
            {
                ModelState.Remove("ProfileUsersData.Password");
                ModelState.Remove("ProfileUsersData.ReTypePassword");
            }

            ModelState.Remove("ProfileUsersData.ProfileSiteData.ProfileName");

            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                TempData["error"] = "Model State Validation Failed.";
                UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"], _profileUserData.Userimgpath);
                if (isProfilePhotoReUpload)
                {
                    ModelState.AddModelError("ProfileUsersData.PhotoFile", "Please upload profile picture again.");
                }
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

                ProfileSite ProfileSiteData = new ProfileSite
                {
                    ProfileId = ProfileUsersData.ProfileSiteData.ProfileId,
                    ProfileName = ProfileUsersData.ProfileSiteData.ProfileName,
                };

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
                    ProfileSiteData = ProfileSiteData,
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
                    UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"], _profileUserData.Userimgpath);
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
