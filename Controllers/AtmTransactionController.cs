using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtmTransactionController : ControllerBase
    {
        private readonly IAtmTransactionRepository _transactionRepository;

        public AtmTransactionController(IAtmTransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime startDate, DateTime endDate)
        {
            return await _transactionRepository.GetAllTransactions(startDate, endDate);
        }
    }
}
