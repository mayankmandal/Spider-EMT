using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Utility;
using System.Text;

namespace Spider_EMT.Pages
{
    // [Authorize]
    public class EditSettingsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ProfileUserAPIVM _userSettings;
        public EditSettingsModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public SettingsVM SettingsData { get; set; } = new SettingsVM();
        public string UserProfilePathUrl = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            await LoadCurrentUserData();
            // Store _userSettings in TempData for subsequent requests
            TempData["UserSettings"] = JsonConvert.SerializeObject(_userSettings);
            return Page();
        }
        private async Task LoadCurrentUserData()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/GetSettingsData");
            _userSettings = JsonConvert.DeserializeObject<ProfileUserAPIVM>(response);

            if (!string.IsNullOrEmpty(_userSettings.Username))
            {
                // Assign values from userSettings to settingsViewModel
                SettingsData = new SettingsVM
                {
                    SettingId = _userSettings.UserId,
                    SettingName = _userSettings.FullName,
                    SettingUsername = _userSettings.Username,
                    SettingEmail = _userSettings.Email,
                    SettingPhotoFile = null,
                };
                UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"], _userSettings.Userimgpath);
            };
        }

        public async Task<IActionResult> OnPost()
        {
            bool isProfilePhotoReUpload = true;
            // Check if _userSettings is already in TempData
            if (TempData.ContainsKey("UserSettings"))
            {
                _userSettings = JsonConvert.DeserializeObject<ProfileUserAPIVM>(TempData["UserSettings"].ToString());
                if (_userSettings.Username == SettingsData.SettingUsername)
                {
                    ModelState.Remove("SettingsData.SettingUsername");
                }

                if (_userSettings.Email == SettingsData.SettingEmail)
                {
                    ModelState.Remove("SettingsData.SettingEmail");
                }
            }

            if (SettingsData.SettingPhotoFile == null)
            {
                ModelState.Remove("SettingsData.SettingPhotoFile");
                isProfilePhotoReUpload = false;
            }

            if (SettingsData.Password == null || SettingsData.ReTypePassword == null)
            {
                ModelState.Remove("SettingsData.Password");
                ModelState.Remove("SettingsData.ReTypePassword");
            }

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Model State Validation Failed.";
                TempData["UserSettings"] = JsonConvert.SerializeObject(_userSettings);
                UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"], _userSettings.Userimgpath);
                if (isProfilePhotoReUpload)
                {
                    ModelState.AddModelError("SettingsData.SettingPhotoFile", "Please upload profile picture again.");
                }
                return Page();
            }

            string uniqueFileName = null;
            string filePath = null;
            string uploadFolder = null;

            if (SettingsData.SettingPhotoFile != null)
            {
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
                TempData["success"] = $"{SettingsData.SettingName} - Profile Updated Successfully";
                TempData.Remove("UserSettings");
                return RedirectToPage();
            }
            else
            {
                TempData["error"] = $"{SettingsData.SettingName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
                TempData["UserSettings"] = JsonConvert.SerializeObject(_userSettings);
                UserProfilePathUrl = Path.Combine(_configuration["UserProfileImgPath"], _userSettings.Userimgpath);
                return Page();
            }
        }
    }
}
