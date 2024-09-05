using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Spider_EMT.Data.Account;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;

namespace Spider_EMT.Middlewares
{
    public class PageAccessRequirement : IAuthorizationRequirement { }
    public class PageAccessHandler : AuthorizationHandler<PageAccessRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PageAccessHandler(ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PageAccessRequirement requirement)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user.ChangePassword != null && user.ChangePassword == true)
            {
                // Set the redirection URL in the HttpContext.Items 
                _httpContextAccessor.HttpContext.Items["RedirectUrl"] = "/Account/ChangePassword";
                context.Fail();
                return;
            }

            var currentPage = _httpContextAccessor.HttpContext.Request.Path.Value;

            // Allow access to the MFA setup page without MFA claim and Login Two Factor With Authenticator
            if (currentPage.Equals(Constants.Page_LoginTwoFactorWithAuthenticator, StringComparison.OrdinalIgnoreCase) || currentPage.Equals(Constants.Page_AuthenticatorWithMFASetup, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return;
            }

            var jwtToken = _currentUserService.GetJWTCookie(Constants.JwtAMRTokenName);
            var principal = _currentUserService.GetPrincipalFromToken(jwtToken);
            if (principal == null)
            {
                context.Fail();
                HandleAccessDenied(context);
                return;
            }

            // Check for MFA Claim in user claims
            var hasMfaClaim = principal.Claims.Any(c => c.Type == Constants.ClaimAMRKey && c.Value == Constants.ClaimAMRValue);
            if (!hasMfaClaim)
            {
                context.Fail();
                HandleAccessDenied(context);
                return;
            }

            user = await _currentUserService.GetCurrentUserAsync();
            // Check if MFA is required and whether the user has completed MFA verfification
            if (IsMFARequired(user) && !IsMFAVerified(user))
            {
                context.Fail();
                RedirectToMFASetup();
                return;
            }

            if (currentPage.Equals(Constants.Page_UserVerificationSetup, StringComparison.OrdinalIgnoreCase))
            {
                // Pass for this screen, screen used for setting IsActive Flag
            }
            else if (user.IsActive == null || user.IsActive == false)
            {
                context.Fail();
                HandleAccessDenied(context);
                return;
            }

            // Try to get pages from session
            var pagesJson = _httpContextAccessor.HttpContext.Session.GetString(SessionKeys.CurrentUserPagesKey);
            List<PageSiteVM> pages = !string.IsNullOrEmpty(pagesJson) ? JsonConvert.DeserializeObject<List<PageSiteVM>>(pagesJson) : null;

            if (pages == null)
            {
                // If pages are not in session, fetch and store them in session
                var accessToken = _currentUserService.GetJWTCookie(Constants.JwtCookieName);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    await _currentUserService.FetchAndCacheUserPermissions(accessToken);
                    pagesJson = _httpContextAccessor.HttpContext.Session.GetString(SessionKeys.CurrentUserPagesKey);
                    if (!string.IsNullOrEmpty(pagesJson))
                    {
                        pages = JsonConvert.DeserializeObject<List<PageSiteVM>>(pagesJson);
                    }
                }
            }

            if (pages != null && pages.Any(page => page.PageUrl.Equals(currentPage, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                HandleAccessDenied(context, currentPage);
            }
        }

        private bool IsMFARequired(ApplicationUser user)
        {
            return !user.TwoFactorEnabled; // MFA is required if Two-Factor Authentication is not enabled
        }

        private bool IsMFAVerified(ApplicationUser user)
        {
            // Check if the user has already completed MFA verification during this session
            return _httpContextAccessor.HttpContext.User.HasClaim(c => c.Type == "amr" && c.Value == "mfa");
        }

        private void RedirectToMFASetup()
        {
            _currentUserService.UserContext.Response.Redirect("/Account/LoginTwoFactorWithAuthenticator");
        }

        private void HandleAccessDenied(AuthorizationHandlerContext context, string currentPage = null)
        {
            context.Fail();
            var isAuthenticated = _currentUserService.UserContext.User.Identity.IsAuthenticated;
            var accessDeniedUrl = isAuthenticated ? "/Account/AccessDenied" : "/Account/Login";


            if (!string.IsNullOrEmpty(currentPage) && isAuthenticated)
            {
                accessDeniedUrl += $"?returnUrl={currentPage}";
            }
            _currentUserService.UserContext.Response.Redirect(accessDeniedUrl);
        }
    }
}
