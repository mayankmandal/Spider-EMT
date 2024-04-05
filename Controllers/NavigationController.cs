using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Route("GetAllProfiles")]
        public IActionResult GetAllProfiles()
        {
            try
            {
                IEnumerable<Profile> allProfilesData = _navigationRepository.GetAllProfiles();
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
                IEnumerable<Page> allPagesData = _navigationRepository.GetAllPages();
                return Ok(allPagesData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
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
        public async Task<IActionResult> AddUserProfile([FromBody] Profile profile)
        {
            try
            {
                if(profile == null)
                {
                    return BadRequest();
                }
                await _navigationRepository.AddUserProfile(profile);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

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
    }
}
