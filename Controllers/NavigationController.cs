using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;

namespace Spider_EMT.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        #region Fields
        private readonly INavigationRepository _navigationRepository;
        private ICacheProvider _cacheProvider;
        #endregion

        #region Constructor
        public NavigationController(INavigationRepository navigationRepository, ICacheProvider cacheProvider)
        {
            _navigationRepository = navigationRepository;
            _cacheProvider = cacheProvider;
        }
        #endregion

        #region Actions

        [HttpGet("GetAllProfiles")]
        [ProducesResponseType(typeof(IEnumerable<ProfileSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProfiles()
        {
            try
            {
                var allProfilesData = await _navigationRepository.GetAllProfilesAsync();
                return Ok(allProfilesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetAllPages")]
        [ProducesResponseType(typeof(IEnumerable<PageSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPages()
        {
            try
            {
                var allPagesData = await _navigationRepository.GetAllPagesAsync();
                return Ok(allPagesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<PageCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var allPageCategoryData = await _navigationRepository.GetAllCategoriesAsync();
                return Ok(allPageCategoryData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUser")]
        [ProducesResponseType(typeof(CurrentUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                if(!_cacheProvider.TryGetValue(CacheKeys.CurrentUserKey, out CurrentUser currentUserData))
                {
                    currentUserData = await _navigationRepository.GetCurrentUserAsync();

                    var cacheEntryOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                        SlidingExpiration = TimeSpan.FromSeconds(10),
                        Size = 1024
                    };

                    _cacheProvider.Set(CacheKeys.CurrentUserKey, currentUserData, cacheEntryOption);
                }
                return Ok(currentUserData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserDetails")]
        [ProducesResponseType(typeof(ProfileUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserDetails()
        {
            try
            {
                var currentUserDetails = await _navigationRepository.GetCurrentUserDetailsAsync();
                return Ok(currentUserDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserProfile")]
        [ProducesResponseType(typeof(ProfileSite), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                if (!_cacheProvider.TryGetValue(CacheKeys.CurrentUserProfileKey, out ProfileSite currentUserProfile))
                {
                    currentUserProfile = await _navigationRepository.GetCurrentUserProfileAsync();

                    var cacheEntryOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                        SlidingExpiration = TimeSpan.FromSeconds(10),
                        Size = 1024
                    };

                    _cacheProvider.Set(CacheKeys.CurrentUserProfileKey, currentUserProfile, cacheEntryOption);
                }
                return Ok(currentUserProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserPages")]
        [ProducesResponseType(typeof(IEnumerable<PageSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserPages()
        {
            try
            {
                if (!_cacheProvider.TryGetValue(CacheKeys.CurrentUserPagesKey, out List<PageSite> pageSites))
                {
                    pageSites = await _navigationRepository.GetCurrentUserPagesAsync();

                    var cacheEntryOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                        SlidingExpiration = TimeSpan.FromSeconds(10),
                        Size = 1024
                    };

                    _cacheProvider.Set(CacheKeys.CurrentUserPagesKey, pageSites, cacheEntryOption);
                }
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetProfilePageData")]
        [ProducesResponseType(typeof(IEnumerable<PageSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfilePageData(string profileId)
        {
            try
            {
                var pageSites = await _navigationRepository.GetProfilePageDataAsync(profileId);
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoriesSetDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserCategories()
        {
            try
            {
                if (!_cacheProvider.TryGetValue(CacheKeys.CurrentUserCategoriesKey, out List<CategoryDisplayViewModel> StructureData))
                {
                    List<CategoriesSetDTO> categoriesSet = await _navigationRepository.GetCurrentUserCategoriesAsync();
                    var groupedCategories = categoriesSet.GroupBy(cat => string.IsNullOrEmpty(cat.CatagoryName) ? Constants.CategoryType_UncategorizedPages : cat.CatagoryName);
                    StructureData = new List<CategoryDisplayViewModel>();

                    foreach (var categoryGroup in groupedCategories)
                    {
                        var category = new CategoryDisplayViewModel
                        {
                            CatagoryName = categoryGroup.Key,
                            Pages = categoryGroup.Select(page => new PageDisplayViewModel
                            {
                                PageDescription = page.PageDescription,
                                PageUrl = page.PageUrl,
                            }).ToList()
                        };
                        StructureData.Add(category);
                    }

                    // Sort the pages within each category
                    foreach (var category in StructureData)
                    {
                        category.Pages = category.Pages.OrderBy(page => page.PageDescription).ToList();
                    }

                    // Sort the categories
                    StructureData = StructureData.OrderBy(cat => cat.CatagoryName).ToList();

                    var cacheEntryOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                        SlidingExpiration = TimeSpan.FromSeconds(10),
                        Size = 1024
                    };

                    _cacheProvider.Set(CacheKeys.CurrentUserCategoriesKey, StructureData, cacheEntryOption);
                }
                return Ok(StructureData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("GetPageToCategories")]
        [ProducesResponseType(typeof(IEnumerable<PageCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPageToCategories([FromBody] List<int> pageList)
        {
            try
            {
                if (pageList == null)
                {
                    return BadRequest();
                }

                var pageCategories = await _navigationRepository.GetPageToCategoriesAsync(pageList);
                return Ok(pageCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("CreateUserAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.CreateUserAccessAsync(profilePagesAccessDTO);
                _cacheProvider.Remove(CacheKeys.CurrentUserProfileKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserPagesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateUserAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.UpdateUserAccessAsync(profilePagesAccessDTO);
                _cacheProvider.Remove(CacheKeys.CurrentUserProfileKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserPagesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("CreateUserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserProfile([FromBody] ProfileUser profileUsersData)
        {
            try
            {
                if (profileUsersData == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.CreateUserProfileAsync(profileUsersData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateUserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ProfileUser profileUsersData)
        {
            try
            {
                if (profileUsersData == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.UpdateUserProfileAsync(profileUsersData);
                _cacheProvider.Remove(CacheKeys.CurrentUserProfileKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserPagesKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserCategoriesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCategoryToPages")]
        [ProducesResponseType(typeof(IEnumerable<PageSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategoryToPages(int categoryId)
        {
            try
            {
                var pages = await _navigationRepository.GetCategoryToPagesAsync(categoryId);
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("CreateNewCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.CreateNewCategoryAsync(categoryPagesAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.UpdateCategoryAsync(categoryPagesAccessDTO);
                _cacheProvider.Remove(CacheKeys.CurrentUserCategoriesKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserPagesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("AssignProfileCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignProfileCategories([FromBody] ProfileCategoryAccessDTO profileCategoryAccessDTO)
        {
            try
            {
                if (profileCategoryAccessDTO == null)
                {
                    return BadRequest();
                }

                // Method Call for assigning categories to profiles
                await _navigationRepository.AssignProfileCategoriesAsync(profileCategoryAccessDTO);

                // Remove the cached item to force a refresh next time
                _cacheProvider.Remove(CacheKeys.CurrentUserCategoriesKey);
                _cacheProvider.Remove(CacheKeys.CurrentUserPagesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("SearchUserDetails")]
        [ProducesResponseType(typeof(IEnumerable<ProfileUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchUserDetails(string criteria, string input)
        {
            try
            {
                if (string.IsNullOrEmpty(criteria) || string.IsNullOrEmpty(input))
                {
                    return BadRequest();
                }

                var profileUsers = await _navigationRepository.SearchUserDetailsByCriteriaAsync(criteria, input);
                return Ok(profileUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        #endregion
    }
}
