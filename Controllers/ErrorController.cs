using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        #region Fields
        private readonly IErrorLogRepository _errorLogRepository;
        #endregion

        #region Constructor
        public ErrorController(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }
        #endregion

        #region Actions
        [HttpPost("LogError")]
        public async Task<IActionResult> LogError([FromBody] ErrorLog errorLog)
        {
            var logId = await _errorLogRepository.LogErrorAsync(errorLog);
            return Ok(logId);
        }
        #endregion
    }
}
