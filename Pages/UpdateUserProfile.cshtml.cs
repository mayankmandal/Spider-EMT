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
        public async Task<JsonResult> OnPost()
        {
            if (ProfileUsersData.PhotoFile == null)
            {
                ModelState.Remove("ProfileUsersData.PhotoFile");
            }

            if (ProfileUsersData.Password == null)
            {
                ModelState.Remove("ProfileUsersData.Password");
            }

            if (!ModelState.IsValid)
            {
                await LoadAllProfilesData(); // Reload ProfilesData if there's a validation error
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
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
                    return new JsonResult(new { success = true, message = $"{ProfileUsersData.FullName} - Profile Updated Successfully" });
                }
                else
                {
                    await LoadAllProfilesData();
                    return new JsonResult(new { success = true, message = $"{ProfileUsersData.FullName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $"{ProfileUsersData.FullName} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
