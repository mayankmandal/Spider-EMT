using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;
using System.Text;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace Spider_EMT.Pages.Account
{
    [Authorize(Policy = "PageAccess")]
    public class UserVerificationSetupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICurrentUserService _currentUserService;

        public UserVerificationSetupModel(IConfiguration configuration, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment, ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _clientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
            _currentUserService = currentUserService;
        }
        [BindProperty]
        public UserVerficationViewModel UserVerficationData { get; set; }
        public string UserProfilePathUrl = string.Empty;
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null || !user.EmailConfirmed || !user.TwoFactorEnabled)
                {
                    TempData["error"] = $"Invalid attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }
                if (user.UserVerificationSetupEnabled == true)
                {
                    return RedirectToPage("/Account/UserRoleAssignment");
                }
                if (user != null)
                {
                    UserVerficationData = new UserVerficationViewModel()
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        IdNumber = user.IdNumber != null ? user.IdNumber : string.Empty,                        
                        MobileNo = user.MobileNo != null ? user.MobileNo : string.Empty,                        
                        Username =  user.UserName != user.Email? user.UserName : string.Empty,
                        PhotoFile = null // Initially set to null for GET request
                    };
                    var userProfileImgPath = _configuration["UserProfileImgPath"];
                    var defaultUserImgPath = _configuration["DefaultUserImgPath"];

                    UserProfilePathUrl = !string.IsNullOrEmpty(user.Userimgpath)? $"/{Path.Combine(userProfileImgPath, user.Userimgpath).Replace("\\", "/")}" : defaultUserImgPath;
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Account/AccessDenied");
                }
                
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            bool isProfilePhotoReUpload = true;

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Model State Validation Failed.";
                UserProfilePathUrl = _configuration["DefaultUserImgPath"];
                if (isProfilePhotoReUpload)
                {
                    ModelState.AddModelError("UserVerficationData.PhotoFile", "Please upload profile picture again.");
                }
                return Page();
            }

            try
            {
                var user = await _currentUserService.GetCurrentUserAsync();
                if (user == null || !user.EmailConfirmed || !user.TwoFactorEnabled)
                {
                    TempData["error"] = $"Invalid attempt. Please check your input and try again.";
                    return RedirectToPage("/Account/Login");
                }
                if (user.UserVerificationSetupEnabled == true)
                {
                    return RedirectToPage("/Account/UserRoleAssignment");
                }

                string uniqueFileName = null;
                string filePath = null;
                string uploadFolder = null;
                if (UserVerficationData.PhotoFile != null)
                {
                    uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"]);
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + UserVerficationData.PhotoFile.FileName;
                    filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // FileStream is properly disposed of after use
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await UserVerficationData.PhotoFile.CopyToAsync(fileStream);
                    }
                    isProfilePhotoReUpload = false;
                }

                UserVerifyApiVM profileUserAPIVM = new UserVerifyApiVM
                {
                    UserId = UserVerficationData.UserId,
                    IdNumber = UserVerficationData.IdNumber,
                    Email = UserVerficationData.Email,
                    MobileNo = UserVerficationData.MobileNo,
                    FullName = UserVerficationData.FullName,
                    Username = UserVerficationData.Username,
                    Userimgpath = uniqueFileName,
                    CreateUserId = UserVerficationData.CreateUserId,
                    UpdateUserId = UserVerficationData.UpdateUserId
                };

                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/UpdateUserVerification";
                var jsonContent = JsonConvert.SerializeObject(profileUserAPIVM);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    _currentUserService.RefreshCurrentUserAsync();
                    TempData["success"] = $"{UserVerficationData.FullName} - Profile Created Successfully";
                    return RedirectToPage("/Account/UserRoleAssignment");
                }
                else
                {
                    TempData["error"] = $"{UserVerficationData.FullName} - Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
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
            TempData["error"] = $"{UserVerficationData.FullName} - " + errorMessage + ". Error details: " + ex.Message;
            return Page();
        }
    }
}
