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
        // Not in Use
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
        // Not in Use
        [HttpGet("GetCurrentProfiles")]
        public IActionResult GetCurrentProfiles()
        {
            try
            {
                IEnumerable<CurrentUserProfileViewModel> allProfiles = _navigationRepository.GetCurrentProfiles();
                return Ok(allProfiles);
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
        // Not in Use
        [HttpPost("AddUserPermissions")]
        public async Task<IActionResult> AddUserPermissions([FromBody] UserPermission userPermission)
        {
            try
            {
                if (userPermission == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.AddUserPermissions(userPermission);
                return Ok();
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
    }
}
