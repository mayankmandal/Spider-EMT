using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSDataController : ControllerBase
    {
        private readonly ISSDataRepository _ssDataRepository;

        public SSDataController(ISSDataRepository ssDataRepository)
        {
            _ssDataRepository = ssDataRepository;
        }

        [HttpGet]
        [Route("GetSsData")]
        public async Task<IEnumerable<SSDataViewModel>> GetSsData()
        {
            return await _ssDataRepository.GetSsData();
        }
    }
}
