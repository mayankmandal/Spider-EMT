using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;

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
        [HttpGet("GetAllPageCategories")]
        public IActionResult GetAllPageCategories() 
        {
            try
            {
                IEnumerable<PageCategory> allPageCategoryData = _navigationRepository.GetAllPageCategories();
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
        [HttpGet("GetCurrentUserCategories")]
        public IActionResult GetCurrentUserCategories()
        {
            try
            {
                List<PageCategory> pageCategories = _navigationRepository.GetCurrentUserCategories();
                return Ok(pageCategories);
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
        [HttpPost("AddUserProfile")]
        public async Task<IActionResult> AddUserProfile([FromBody] UserProfileDTO userProfileDTO)
        {
            try
            {
                if(userProfileDTO.Profile.ProfileId != 0 || userProfileDTO.Profile.ProfileName.IsNullOrEmpty())
                {
                    return BadRequest("Invalid Request Payload");
                }
                await _navigationRepository.AddUserProfile(userProfileDTO.Profile, userProfileDTO.Pages, userProfileDTO.PageCategories);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDTO userProfileDTO)
        {
            try
            {
                if (userProfileDTO.Profile.ProfileId != 0 || userProfileDTO.Profile.ProfileName.IsNullOrEmpty())
                {
                    await _navigationRepository.UpdateUserProfile(userProfileDTO.Profile, userProfileDTO.Pages, userProfileDTO.PageCategories);
                    return Ok();
                }
                return BadRequest("Invalid Request Payload");
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
        [HttpPost("AddNewUserProfile")]
        public async Task<IActionResult> AddNewUserProfile([FromBody] UserToProfileDTO userToProfileDTO)
        {
            try
            {
                if (userToProfileDTO == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.AddNewUserProfile(userToProfileDTO.Profiles, userToProfileDTO.ProfileUsers);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
