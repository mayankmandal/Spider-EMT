using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using QRCoder;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Text;

namespace Spider_EMT.Pages.Account
{
    [AllowAnonymous]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpClientFactory _clientFactory;
        [BindProperty]
        public SetupMFAViewModel setupMFAVM { get; set; }
        [BindProperty]
        public bool Succeeded { get; set; }
        public AuthenticatorWithMFASetupModel(IConfiguration configuration, ICurrentUserService currentUserService, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
            setupMFAVM = new SetupMFAViewModel();
            Succeeded = false;
            _clientFactory = clientFactory;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                TempData["error"] = "User not found.";
                ModelState.AddModelError("AuthenticatorSetup", "User not found.");
                return RedirectToPage("/Account/Login");
            }
            if (!user.EmailConfirmed)
            {
                TempData["error"] = "Please confirm your email before setting up two-factor authentication.";
                return RedirectToPage("/Account/Login");
            }
            if (user.TwoFactorEnabled)
            {
                return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator");
            }
            var key = await _currentUserService.UserManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _currentUserService.UserManager.ResetAuthenticatorKeyAsync(user);
                key = await _currentUserService.UserManager.GetAuthenticatorKeyAsync(user);
            }
            setupMFAVM.Key = key;
            setupMFAVM.QRCodeBytes = GenerateQRCodeBytes(_configuration["MFAAuthenticatorSetupProvider"], key, user.Email);

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid input. Please check your data and try again.";
                return Page();
            }
            var user = await _currentUserService.GetCurrentUserAsync();

            if (user == null)
            {
                TempData["error"] = "User not found.";
                ModelState.AddModelError("AuthenticatorSetup", "User not found.");
                return Page();
            }

            if (await _currentUserService.UserManager.VerifyTwoFactorTokenAsync(user, _currentUserService.UserManager.Options.Tokens.AuthenticatorTokenProvider, setupMFAVM.SecurityCode))
            {
                var result = await _currentUserService.UserManager.SetTwoFactorEnabledAsync(user, true);
                if (result.Succeeded)
                {
                    _currentUserService.RefreshCurrentUserAsync();
                    Succeeded = true;
                    
                    // API call
                    var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWTCookieHelper.GetJWTCookie(HttpContext));
                    var apiUrl = $"{_configuration["ApiBaseUrl"]}/Navigation/CreateBaseUserAccess";
                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = $"Two-factor authentication has been set up successfully and base access role is provided to user to {user.FullName}";
                        return RedirectToPage("/Account/UserVerificationSetup");
                    }
                    else
                    {
                        TempData["success"] = "Two-factor authentication has been set up successfully.";
                        TempData["error"] = $"{user.FullName} - Error occurred on providing base access role in response with status: {response.StatusCode} - {response.ReasonPhrase}";
                        return Page();
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("SetupMFAViewModel", error.Description);
                    }
                    return Page();
                }
            }
            else
            {
                TempData["error"] = "Invalid security code. Please try again.";
                ModelState.AddModelError("AuthenticatorSetup", "Invalid security code. Please try again.");
                return Page();
            }
        }
        private Byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth:/totp{provider}:{userEmail}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return BitmapToByteArray(qrCodeImage);
        }
        private Byte[] BitmapToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
