using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankRepository _bankRepository;
        public BankController(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        [HttpGet]
        [Route("GetBanks")]
        public async Task<IEnumerable<BankReferenceData>> GetBanks()
        {
            return await _bankRepository.GetBanks();
        }
    }
}
