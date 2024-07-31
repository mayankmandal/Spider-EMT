using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Data.Account;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;
using System.Drawing;
using System.Drawing.Imaging;

namespace Spider_EMT.Pages.Account
{
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;
        [BindProperty]
        public SetupMFAViewModel setupMFAVM { get; set; }
        [BindProperty]
        public bool Succeeded { get; set; }
        public AuthenticatorWithMFASetupModel(IConfiguration configuration, ICurrentUserService currentUserService)
        {
            _configuration = configuration;
            _currentUserService = currentUserService;
            setupMFAVM = new SetupMFAViewModel();
            Succeeded = false;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _currentUserService.UserManager.GetUserAsync(base.User);
            if(user == null)
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
            setupMFAVM.QRCodeBytes = GenerateQRCodeBytes(_configuration["MFAAuthenticatorSetupProvider"],key, user.Email);

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid input. Please check your data and try again.";
                return Page();
            }
            var user = await _currentUserService.UserManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                TempData["error"] = "User not found.";
                ModelState.AddModelError("AuthenticatorSetup", "User not found.");
                return Page();
            }

            if (await _currentUserService.UserManager.VerifyTwoFactorTokenAsync(user, _currentUserService.UserManager.Options.Tokens.AuthenticatorTokenProvider, setupMFAVM.SecurityCode))
            {
                await _currentUserService.UserManager.SetTwoFactorEnabledAsync(user, true);
                Succeeded = true;
                TempData["success"] = "Two-factor authentication has been set up successfully.";
                return RedirectToPage("/Account/UserVerficationSetup");
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
