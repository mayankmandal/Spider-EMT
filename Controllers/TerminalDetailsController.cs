using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalDetailsController : ControllerBase
    {
        private readonly ITerminalDetailsRepository _terminalDetailsRepository;
        public TerminalDetailsController(ITerminalDetailsRepository terminalDetailsRepository)
        {
            _terminalDetailsRepository = terminalDetailsRepository;
        }

        [HttpGet]
        [Route("GetTerminalDetails/{terminalId}")]
        public async Task<TerminalDetails> GetTerminalDetails(string terminalId)
        {
            return await _terminalDetailsRepository.GetTerminalDetails(terminalId);
        }
    }
}
