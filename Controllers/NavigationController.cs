using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Diagnostics;

namespace Spider_EMT.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationRepository _navigationRepository;
        public NavigationController(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }
        [HttpGet]
        [Route("GetCurrentUser")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                CurrentUser currentUserData = _navigationRepository.GetCurrentUser();
                return Ok(currentUserData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("GetAllProfileUsers")]
        public IActionResult GetAllProfileUsers()
        {
            try
            {
                IEnumerable<ProfileUser> allProfilesData = _navigationRepository.GetAllProfileUsers();
                return Ok(allProfilesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("GetAllProfiles")]
        public IActionResult GetAllProfiles()
        {
            try
            {
                IEnumerable<ProfileSite> allProfilesData = _navigationRepository.GetAllProfiles();
                return Ok(allProfilesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetAllPages")]
        public IActionResult GetAllPages()
        {
            try
            {
                IEnumerable<PageSite> allPagesData = _navigationRepository.GetAllPages();
                return Ok(allPagesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        // Not in Use
        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories() 
        {
            try
            {
                IEnumerable<PageCategory> allPageCategoryData = _navigationRepository.GetAllCategories();
                return Ok(allPageCategoryData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetCurrentUserProfile")]
        public IActionResult GetCurrentUserProfile()
        {
            try
            {
                ProfileSite currentUserProfile = _navigationRepository.GetCurrentUserProfile();
                return Ok(currentUserProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetCurrentUserPages")]
        public IActionResult GetCurrentUserPages()
        {
            try
            {
                List<PageSite> pageSites = _navigationRepository.GetCurrentUserPages();
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetProfilePageData")]
        public IActionResult GetProfilePageData(string profileId)
        {
            try
            {
                List<PageSite> pageSites = _navigationRepository.GetProfilePageData(profileId);
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetCurrentUserCategories")]
        public IActionResult GetCurrentUserCategories()
        {
            try
            {
                List<CategoriesSetDTO> categoriesSet = _navigationRepository.GetCurrentUserCategories();
                return Ok(categoriesSet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetNewUserPages")]
        public async Task<IActionResult> GetNewUserPages()
        {
            try
            {
                List<PageSite> pageSites = _navigationRepository.GetNewUserPages();
                return Ok(pageSites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetNewUserCategories")]
        public async Task<IActionResult> GetNewUserCategories()
        {
            try
            {
                List<PageCategory> pageCategories = _navigationRepository.GetNewUserCategories();
                return Ok(pageCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("GetPageToCategories")]
        public async Task<IActionResult> GetPageToCategories([FromBody] List<int> pageList)
        {
            try
            {
                if(pageList == null)
                {
                    return BadRequest();
                }
                List<PageCategory> pageCategories = new List<PageCategory>();
                pageCategories = _navigationRepository.GetPageToCategories(pageList);
                return Ok(pageCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("CreateUserAccess")]
        public async Task<IActionResult> CreateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.CreateUserAccess(profilePagesAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("UpdateUserAccess")]
        public async Task<IActionResult> UpdateUserAccess([FromBody] ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                if (profilePagesAccessDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.UpdateUserAccess(profilePagesAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("CreateUserProfile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] ProfileUser profileUsersData)
        {
            try
            {
                if (profileUsersData == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.CreateUserProfile(profileUsersData);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("GetCategoryToPages")]
        public async Task<IActionResult> GetCategoryToPages(int categoryId)
        {
            try
            {
                if (categoryId == null)
                {
                    return BadRequest();
                }
                List<PageSite> pages = new List<PageSite>();
                pages = _navigationRepository.GetCategoryToPages(categoryId);
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("CreateNewCategory")]
        public async Task<IActionResult> CreateNewCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.CreateNewCategory(categoryPagesAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                if (categoryPagesAccessDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.UpdateCategory(categoryPagesAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPost("AssignProfileCategories")]
        public async Task<IActionResult> AssignProfileCategories([FromBody] ProfileCategoryAccessDTO profileCategoryAccessDTO)
        {
            try
            {
                if (profileCategoryAccessDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.AssignProfileCategories(profileCategoryAccessDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
