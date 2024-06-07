using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace Spider_EMT.Pages
{
    public class EditSettingsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EditSettingsModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public SettingsViewModel SettingsData { get; set; } = new SettingsViewModel();
        public string UserProfilePathUrl = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadCurrentUserData();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        private async Task LoadCurrentUserData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetSettingsData");
            var userSettings = JsonConvert.DeserializeObject<UserSettings>(response);

            if (!string.IsNullOrEmpty(userSettings.Username))
            {
                // Assign values from userSettings to settingsViewModel
                SettingsData = new SettingsViewModel
                {
                    SettingId = userSettings.UserId,
                    SettingName = userSettings.FullName,
                    SettingUsername = userSettings.Username,
                    SettingEmail = userSettings.EmailAddress,
                    SettingPhotoFile = null,
                };
                UserProfilePathUrl = "/images/profiles_picture/" + userSettings.ProfilePhotoPath;
            };
        }

        public async Task<JsonResult> OnPostSubmitAsync()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
            }
            try
            {
                string uniqueFileName = null;
                string filePath = null;
                string uploadFolder = null;
                if (SettingsData.SettingPhotoFile != null)
                {
                    uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profiles_picture");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + SettingsData.SettingPhotoFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // FileStream is properly disposed of after use
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await SettingsData.SettingPhotoFile.CopyToAsync(fileStream);
                    }
                }

                UserSettings userSettings = new UserSettings
                {
                    UserId = SettingsData.SettingId,
                    FullName = SettingsData.SettingName,
                    EmailAddress = SettingsData.SettingEmail,
                    ProfilePhotoPath = uniqueFileName,
                    Username = SettingsData.SettingUsername,
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateSettingsData";
                var jsonContent = JsonConvert.SerializeObject(userSettings);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $" - Profile Updated Successfully" });
                }
                else
                {
                    return new JsonResult(new { success = true, message = $" - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $" - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
