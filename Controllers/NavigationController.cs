using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Spider_EMT.Utility.Constants;

namespace Spider_EMT.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NavigationController : ControllerBase
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INavigationRepository _navigationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$";
        #endregion

        #region ,
        public NavigationController(IHttpContextAccessor httpContextAccessor, INavigationRepository navigationRepository, ICurrentUserService currentUserService, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _navigationRepository = navigationRepository;
            _currentUserService = currentUserService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        #endregion

        #region Actions

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    // Ensure any operations on the file are completed before deletion
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose))
                    {
                        // This will ensure the file is closed properly before deletion
                        stream.Close();
                    }

                    // Delete the file after ensuring it is not being used by another process
                    System.IO.File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting the file.", ex);
            }
        }

        [HttpGet("GetAllUsers")]
        [ProducesResponseType(typeof(IEnumerable<ProfileUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var allUsersData = await _navigationRepository.GetAllUsersDataAsync();
                foreach (var profileUserAPIVM in allUsersData)
                {
                    profileUserAPIVM.Userimgpath = Path.Combine(_configuration["UserProfileImgPath"], profileUserAPIVM.Userimgpath);
                }
                return Ok(allUsersData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetAllProfiles")]
        [ProducesResponseType(typeof(IEnumerable<ProfileSite>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProfiles()
        {
            try
            {
                List<ProfileSiteVM> profileSiteVMs = new List<ProfileSiteVM>();
                var allProfilesData = await _navigationRepository.GetAllProfilesAsync();
                foreach (var profileSiteData in allProfilesData)
                {
                    ProfileSiteVM profileSite = new ProfileSiteVM
                    {
                        ProfileId = profileSiteData.ProfileId,
                        ProfileName = profileSiteData.ProfileName,
                    };
                    profileSiteVMs.Add(profileSite);
                }
                return Ok(profileSiteVMs);
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
                List<PageSiteVM> pageSiteVMs = new List<PageSiteVM>();
                var allPagesData = await _navigationRepository.GetAllPagesAsync();
                foreach (var page in allPagesData)
                {
                    PageSiteVM pageSiteVM = new PageSiteVM
                    {
                        PageId = page.PageId,
                        isSelected = page.isSelected,
                        PageDescription = page.PageDescription,
                        PageUrl = page.PageUrl
                    };
                    pageSiteVMs.Add(pageSiteVM);
                }
                return Ok(pageSiteVMs);
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
                List<PageCategoryVM> pageCategoryVMs = new List<PageCategoryVM>();
                var allPageCategoryData = await _navigationRepository.GetAllCategoriesAsync();
                foreach (var pageCategoryData in allPageCategoryData)
                {
                    PageCategoryVM pageCategoryVM = new PageCategoryVM
                    {
                        CategoryName = pageCategoryData.CategoryName,
                        PageCatId = pageCategoryData.PageCatId,
                        PageId = pageCategoryData.PageId,
                    };
                    pageCategoryVMs.Add(pageCategoryVM);
                }
                return Ok(pageCategoryVMs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUser")]
        [ProducesResponseType(typeof(CurrentUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var currentUserData = await _currentUserService.GetCurrentUserAsync(jwtToken);

                if (currentUserData == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                CurrentUser currentUser = new CurrentUser
                {
                    UserId = currentUserData.Id,
                    UserImgPath = Path.Combine(_configuration["UserProfileImgPath"], currentUserData.Userimgpath == null? string.Empty:currentUserData.Userimgpath),
                    UserName = currentUserData.UserName,
                };

                return Ok(currentUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserDetails")]
        [ProducesResponseType(typeof(ProfileUserAPIVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserDetails()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                var currentUserDetails = await _navigationRepository.GetCurrentUserDetailsAsync(userId);
                currentUserDetails.Userimgpath = Path.Combine(_configuration["UserProfileImgPath"], currentUserDetails.Userimgpath);
                return Ok(currentUserDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserProfile")]
        [ProducesResponseType(typeof(ProfileSite), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                ProfileSite currentUserProfile = null;
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (!_httpContextAccessor.HttpContext.Session.TryGetValue(SessionKeys.CurrentUserProfileKey, out byte[] currentUserProfileData))
                {
                    currentUserProfile = await _navigationRepository.GetCurrentUserProfileAsync(userId);
                    _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserProfileKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentUserProfile)));
                }
                else
                {
                    currentUserProfile = JsonConvert.DeserializeObject<ProfileSite>(Encoding.UTF8.GetString(currentUserProfileData));
                }
                return Ok(currentUserProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserPages")]
        [ProducesResponseType(typeof(IEnumerable<PageSiteVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserPages()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ","");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                List<PageSiteVM> pageSites = null;

                if (!_httpContextAccessor.HttpContext.Session.TryGetValue(SessionKeys.CurrentUserPagesKey, out byte[] pageSitesData))
                {
                    pageSites = await _navigationRepository.GetCurrentUserPagesAsync(userId);

                    _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserPagesKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(pageSites)));
                }
                else
                {
                    pageSites = JsonConvert.DeserializeObject<List<PageSiteVM>>(Encoding.UTF8.GetString(pageSitesData));
                }
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetCurrentUserCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoriesSetDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUserCategories()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                List<CategoryDisplayViewModel> StructureData;
                if (!_httpContextAccessor.HttpContext.Session.TryGetValue(SessionKeys.CurrentUserCategoriesKey, out byte[] structureDataBytes))
                {
                    List<CategoriesSetDTO> categoriesSet = await _navigationRepository.GetCurrentUserCategoriesAsync(userId);
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

                    _httpContextAccessor.HttpContext.Session.Set(SessionKeys.CurrentUserCategoriesKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(StructureData)));
                }
                else
                {
                    StructureData = JsonConvert.DeserializeObject<List<CategoryDisplayViewModel>>(Encoding.UTF8.GetString(structureDataBytes));
                }
                return Ok(StructureData);
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

        [HttpPost("CreateUserAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }
                bool isSuccess = false;
                isSuccess = await _navigationRepository.CreateUserAccessAsync(profilePagesAccessDTO, userId);

                

                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
                return Ok(isSuccess);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateUserAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.UpdateUserAccessAsync(profilePagesAccessDTO, userId);

                
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserProfile([FromBody] ProfileUserAPIVM profileUsersData)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (profileUsersData == null)
                {
                    return BadRequest();
                }

                // Validate the password
                if (!string.IsNullOrEmpty(profileUsersData.Password) && !Regex.IsMatch(profileUsersData.Password, passwordPattern))
                {
                    return BadRequest("Password must be between 8 and 16 characters, and must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                }
                string hashedPassword = null;

                ProfileUser profileUser = new ProfileUser
                {
                    IdNumber = profileUsersData.IdNumber.ToString(),
                    FullName = profileUsersData.FullName,
                    Email = profileUsersData.Email,
                    Username = profileUsersData.Username,
                    Userimgpath = profileUsersData.Userimgpath,
                    MobileNo = profileUsersData.MobileNo.ToString(),
                    UserId = 0,
                    PasswordHash = hashedPassword,
                    ProfileSiteData = new ProfileSite
                    {
                        ProfileId = profileUsersData.ProfileSiteData.ProfileId,
                    },
                    IsActive = profileUsersData.IsActive,
                    ChangePassword = profileUsersData.ChangePassword,
                    IsActiveDirectoryUser = profileUsersData.IsActiveDirectoryUser,
                };

                await _navigationRepository.CreateUserProfileAsync(profileUser, userId);

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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ProfileUserAPIVM profileUserAPIVM)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var user = await _currentUserService.GetCurrentUserAsync(jwtToken);
                if (user == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (profileUserAPIVM == null || string.IsNullOrEmpty(profileUserAPIVM.Email))
                {
                    return BadRequest("Invalid user profile data");
                }

                // Fetch target user by email from profileUserAPIVM
                var targetUser = await _currentUserService.GetUserByEmailAsync(profileUserAPIVM.Email);
                if(targetUser == null)
                {
                    return NotFound($"User with email {profileUserAPIVM.Email} not found.");
                }

                // Fetch the role of the target user
                var targetUserRole = await _currentUserService.GetUserRoleAsync(profileUserAPIVM.Email);
                if (targetUserRole == null)
                {
                    return NotFound("Target user does not have an associated role.");
                }

                bool isSuccess = false;
                
                if (user.RoleAssignmentEnabled == true && targetUserRole.RoleName == BaseUserRoleName && profileUserAPIVM.ProfileSiteData.ProfileName != targetUserRole.RoleName)
                {
                    var allPagesData = await _navigationRepository.GetAllPagesAsync();
                    List<PageSiteVM> pageSiteVMs = new List<PageSiteVM>();
                    // Get all constant values from the BaseUserScreenAccess static class
                    var baseUserScreenAccessType = typeof(BaseUserScreenAccess);
                    var constants = baseUserScreenAccessType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where(f => f.IsLiteral && !f.IsInitOnly)
                        .Select(f => f.GetValue(null) as string)
                        .ToList();
                    foreach (var page in allPagesData)
                    {
                        PageSiteVM pageSiteVM = new PageSiteVM
                        {
                            PageId = page.PageId,
                            isSelected = page.isSelected,
                            PageDescription = page.PageDescription,
                            PageUrl = page.PageUrl
                        };

                        // Check if the PageDescription is in any of the constant values
                        if (constants.Contains(pageSiteVM.PageUrl))
                        {
                            pageSiteVMs.Add(pageSiteVM);
                        }
                    }

                    ProfilePagesAccessDTO profilePagesAccessDTO = new ProfilePagesAccessDTO
                    {
                        PagesList = pageSiteVMs,
                        ProfileData = profileUserAPIVM.ProfileSiteData
                    };

                    isSuccess = await _navigationRepository.CreateUserAccessAsync(profilePagesAccessDTO, user.Id);
                }
                if (isSuccess)
                {
                    string PreviousProfilePhotoPath = await _navigationRepository.UpdateUserProfileAsync(profileUserAPIVM, user.Id);
                    
                    // Remove the cached item to force a refresh next time
                    _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);
                    _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
                    _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserCategoriesKey);

                    if (!string.IsNullOrEmpty(profileUserAPIVM.Userimgpath) && !string.IsNullOrEmpty(PreviousProfilePhotoPath))
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"], PreviousProfilePhotoPath);
                        bool isSucess = await DeleteFileAsync(oldFilePath);
                        return Ok(isSucess);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.CreateNewCategoryAsync(categoryPagesAccessDTO, userId);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }

                await _navigationRepository.UpdateCategoryAsync(categoryPagesAccessDTO, userId);
                
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserCategoriesKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignProfileCategories([FromBody] ProfileCategoryAccessDTO profileCategoryAccessDTO)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (profileCategoryAccessDTO == null)
                {
                    return BadRequest();
                }

                // Method Call for assigning categories to profiles
                await _navigationRepository.AssignProfileCategoriesAsync(profileCategoryAccessDTO, userId);
                
                // Remove the cached item to force a refresh next time
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserCategoriesKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("SearchUserDetails")]
        [ProducesResponseType(typeof(IEnumerable<ProfileUserAPIVM>), StatusCodes.Status200OK)]
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
                foreach (var userAPIVM in profileUsers)
                {
                    if(!string.IsNullOrEmpty(userAPIVM.Userimgpath))
                    {
                        userAPIVM.Userimgpath = Path.Combine(_configuration["UserProfileImgPath"], userAPIVM.Userimgpath);
                    }
                    else
                    {
                        userAPIVM.Userimgpath = _configuration["DefaultUserImgPath"];
                    }
                }
                return Ok(profileUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteEntity")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntity(int deleteId, string deleteType)
        {
            try
            {
                if (deleteId <= 0 || string.IsNullOrEmpty(deleteType))
                {
                    return BadRequest();
                }

                bool isSuccess = await _navigationRepository.DeleteEntityAsync(deleteId, deleteType);
                return Ok(isSuccess);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetSettingsData")]
        [ProducesResponseType(typeof(ProfileUserAPIVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSettingsData()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                ProfileUserAPIVM profileUserAPIVM = await _navigationRepository.GetSettingsDataAsync(userId);
                return Ok(profileUserAPIVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateSettingsData")]
        [ProducesResponseType(typeof(SettingsAPIVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSettingsData(SettingsAPIVM userSettings)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                // Validate the password
                if (!string.IsNullOrEmpty(userSettings.SettingsPassword) && (!Regex.IsMatch(userSettings.SettingsPassword, passwordPattern) || userSettings.SettingsPassword != userSettings.SettingsReTypePassword))
                {
                    return BadRequest("Password must be between 8 and 16 characters, and must contain at least one uppercase letter, one lowercase letter, one digit, and one special character. Also, both passwords must match.");
                }

                string hashedPassword = null;
                if (!string.IsNullOrEmpty(userSettings.SettingsPassword) || !string.IsNullOrEmpty(userSettings.SettingsReTypePassword))
                {
                    var user = await _currentUserService.GetCurrentUserAsync();
                    if(user != null)
                    {
                        hashedPassword =  _currentUserService.UserManager.PasswordHasher.HashPassword(user,userSettings.SettingsPassword);
                    }
                }

                ProfileUser profileUser = new ProfileUser
                {
                    FullName = userSettings.Name,
                    Email = userSettings.Email,
                    Username = userSettings.Username,
                    Userimgpath = userSettings.PhotoFile,
                    UserId = userSettings.Id,
                    PasswordHash = hashedPassword != null ? hashedPassword : string.Empty,
                };

                string PreviousProfilePhotoPath = await _navigationRepository.UpdateSettingsDataAsync(profileUser, userId);
                
                // Remove the cached item to force a refresh next time
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);

                if (!string.IsNullOrEmpty(profileUser.Userimgpath) && !string.IsNullOrEmpty(PreviousProfilePhotoPath))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"], PreviousProfilePhotoPath);
                    bool isSucess = await DeleteFileAsync(oldFilePath);
                    return Ok(isSucess);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("CheckUniqueness")]
        [AllowAnonymous] // Allow access without authentication
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckUniqueness([FromBody] UniquenessCheckRequest uniqueRequest)
        {
            try
            {
                bool isUniqueValue = false;
                isUniqueValue = await _navigationRepository.CheckUniquenessAsync(uniqueRequest.Field, uniqueRequest.Value);
                return Ok(new { IsUnique = isUniqueValue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("FetchUserRecord")]
        [ProducesResponseType(typeof(ProfileUserAPIVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FetchUserRecord([FromBody] UserIdRequest request)
        {
            try
            {
                ProfileUserAPIVM profileUserAPIVM = await _navigationRepository.GetUserRecordAsync(request.UserId);
                return Ok(profileUserAPIVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("UpdateUserVerification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserVerification([FromBody] UserVerifyApiVM userVerifyApiVM)
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                if (userVerifyApiVM == null)
                {
                    return BadRequest();
                }

                string PreviousProfilePhotoPath = await _navigationRepository.UpdateUserVerificationAsync(userVerifyApiVM, userId);
                
                // Remove the cached item to force a refresh next time
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserCategoriesKey);

                if (!string.IsNullOrEmpty(userVerifyApiVM.Userimgpath) && !string.IsNullOrEmpty(PreviousProfilePhotoPath))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, _configuration["UserProfileImgPath"], PreviousProfilePhotoPath);
                    bool isSucess = await DeleteFileAsync(oldFilePath);
                    return Ok(isSucess);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("CreateBaseUserAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBaseUserAccess()
        {
            try
            {
                var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized("JWT Token is missing");
                }
                var userId = await _currentUserService.GetCurrentUserIdAsync(jwtToken);
                if (userId == null)
                {
                    return Unauthorized("User is not authenticated.");
                }
                bool isSuccess = false;
                isSuccess = await _navigationRepository.CreateBaseUserAccessAsync(BaseUserRoleName, userId);
                
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserProfileKey);
                _httpContextAccessor.HttpContext.Session.Remove(SessionKeys.CurrentUserPagesKey);
                return Ok(isSuccess);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        #endregion
    }
}
