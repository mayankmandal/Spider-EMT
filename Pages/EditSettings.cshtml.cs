using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using System.Text;
using Spider_EMT.Utility;

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
        public SettingsVM SettingsData { get; set; } = new SettingsVM();
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
            var userSettings = JsonConvert.DeserializeObject<ProfileUserAPIVM>(response);

            if (!string.IsNullOrEmpty(userSettings.Username))
            {
                // Assign values from userSettings to settingsViewModel
                SettingsData = new SettingsVM
                {
                    SettingId = userSettings.UserId,
                    SettingName = userSettings.FullName,
                    SettingUsername = userSettings.Username,
                    SettingEmail = userSettings.Email,
                    SettingPhotoFile = null,
                };
                UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"],userSettings.Userimgpath);
            };
        }

        public async Task<JsonResult> OnPostSubmitAsync()
        {
            if (SettingsData.SettingPhotoFile == null)
            {
                ModelState.Remove("SettingsData.SettingPhotoFile");
            }

            if (SettingsData.Password == null || SettingsData.ReTypePassword == null)
            {
                ModelState.Remove("SettingsData.Password");
                ModelState.Remove("SettingsData.ReTypePassword");
            }

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
                    var fileExtension = Path.GetExtension(SettingsData.SettingPhotoFile.FileName).ToLower();
                    if (!Constants.validImageExtensions.Contains(fileExtension))
                    {
                        return new JsonResult(new { success = false, message = "Invalid file type. Only image files (jpg, jpeg, png, gif) are allowed." });
                    }

                    uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"]);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + SettingsData.SettingPhotoFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // FileStream is properly disposed of after use
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await SettingsData.SettingPhotoFile.CopyToAsync(fileStream);
                    }
                }

                SettingsAPIVM userSettings = new SettingsAPIVM
                {
                    Id = SettingsData.SettingId,
                    Name = SettingsData.SettingName,
                    Email = SettingsData.SettingEmail,
                    PhotoFile = SettingsData.SettingPhotoFile != null ? uniqueFileName : "",
                    Username = SettingsData.SettingUsername,
                    SettingsPassword = SettingsData.Password != null ? SettingsData.Password : "",
                    SettingsReTypePassword = SettingsData.ReTypePassword != null ? SettingsData.ReTypePassword : ""
                };

                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateSettingsData";
                var jsonContent = JsonConvert.SerializeObject(userSettings);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = $"{SettingsData.SettingName} - Profile Updated Successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = $"{SettingsData.SettingName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}" });
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
            return new JsonResult(new { success = false, message = $"{SettingsData.SettingName} - " + errorMessage + ". Error details: " + ex.Message });
        }
    }
}
