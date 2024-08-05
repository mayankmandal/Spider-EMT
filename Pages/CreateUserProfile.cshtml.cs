using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Net.Http.Headers;
using System.Text;

namespace Spider_EMT.Pages
{
    [Authorize(Policy = "PageAccess")]
    public class CreateUserProfileModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateUserProfileModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
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
                UserProfilePathUrl = _configuration["DefaultUserImgPath"];
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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetAllProfiles");
            ProfilesData = JsonConvert.DeserializeObject<List<ProfileSiteVM>>(response);
        }
        public async Task<IActionResult> OnPost()
        {
            bool isProfilePhotoReUpload = true;
            ModelState.Remove("ProfileUsersData.ProfileSiteData.ProfileName");

            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                TempData["error"] = "Model State Validation Failed.";
                UserProfilePathUrl = _configuration["DefaultUserImgPath"];
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
                    isProfilePhotoReUpload = false;
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
                    Password = ProfileUsersData.Password,
                    Username = ProfileUsersData.Username,
                    Userimgpath = uniqueFileName,
                    ProfileSiteData = ProfileSiteData,
                    IsActive = ProfileUsersData.IsActive,
                    IsActiveDirectoryUser = ProfileUsersData.IsActiveDirectoryUser,
                    ChangePassword = ProfileUsersData.ChangePassword,
                    CreateUserId = ProfileUsersData.CreateUserId,
                    UpdateUserId = ProfileUsersData.UpdateUserId
                };

                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/CreateUserProfile";
                var jsonContent = JsonConvert.SerializeObject(profileUserAPIVM);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = $"{ProfileUsersData.FullName} - Profile Created Successfully";
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
