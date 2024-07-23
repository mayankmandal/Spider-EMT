using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Data.Account;
using System.Drawing;
using System.Drawing.Imaging;

namespace Spider_EMT.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty] 
        public SetupMFAViewModel setupMFAVM { get; set; }
        [BindProperty]
        public bool Succeeded { get; set; }
        public AuthenticatorWithMFASetupModel(IConfiguration configuration,UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            setupMFAVM = new SetupMFAViewModel();
            Succeeded = false;
        }
        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(base.User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            setupMFAVM.Key = key;
            setupMFAVM.QRCodeBytes = GenerateQRCodeBytes(_configuration["MFAAuthenticatorSetupProvider"],key, user.Email);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(base.User);
            if(await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, setupMFAVM.SecurityCode))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                Succeeded = true;
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with authenticator setup.");
            }
            return Page();
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
