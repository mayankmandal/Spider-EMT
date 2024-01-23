using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentBankDetailsController : ControllerBase
    {
        private readonly ICurrentBankDetailsRepository _currentBankDetailsRepository;

        public CurrentBankDetailsController(ICurrentBankDetailsRepository currentBankDetailsRepository)
        {
            _currentBankDetailsRepository = currentBankDetailsRepository;
        }

        [HttpGet]
        [Route("GetCurrentBankDetails")]
        public async Task<CurrentBankDetails> GetCurrentBankDetails()
        {
            return await _currentBankDetailsRepository.GetCurrentBankDetails();
        }
    }
}
