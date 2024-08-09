using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;

namespace Spider_EMT.Middlewares
{
    public class PageAccessRequirement : IAuthorizationRequirement { }
    public class PageAccessHandler : AuthorizationHandler<PageAccessRequirement>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemoryCache _cacheProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PageAccessHandler(IMemoryCache cacheProvider, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _currentUserService = currentUserService;
            _cacheProvider = cacheProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PageAccessRequirement requirement)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                context.Fail();
                return;
            }
            if (!_cacheProvider.TryGetValue(CacheKeys.CurrentUserPagesKey, out List<PageSiteVM> pages))
            {
                var accessToken = _currentUserService.GetJWTCookie();
                if (!string.IsNullOrEmpty(accessToken))
                {
                     await _currentUserService.FetchAndCacheUserPermissions(accessToken);
                    _cacheProvider.TryGetValue(CacheKeys.CurrentUserPagesKey, out pages);
                }
            }

            var currentPage = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (pages != null && pages.Any(page => page.PageUrl.Equals(currentPage, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                HandleAccessDenied(context, currentPage);
            }
        }
        private void HandleAccessDenied(AuthorizationHandlerContext context, string currentPage = null)
        {
            context.Fail();
            if (_currentUserService.UserContext.User.Identity.IsAuthenticated)
            {
                var accessDeniedUrl = "/Account/AccessDenied";
                if (!string.IsNullOrEmpty(currentPage))
                {
                    accessDeniedUrl += $"?returnUrl={currentPage}";
                }
                _currentUserService.UserContext.Response.Redirect(accessDeniedUrl);
            }
            else
            {
                _currentUserService.UserContext.Response.Redirect("/Account/Login");
            }
        }
    }
}
