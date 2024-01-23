using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;
using System.Reflection;


namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransactionSummaryController : ControllerBase
    {
        private readonly IBankTransactionSummaryRepository _bankTransactionSummaryRepository;
        private readonly IAtmTransactionRepository _transactionRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ITransactionFeeRepository _transactionFeeRepository;
        private readonly ISSDataRepository _ssDataRepository;
        private readonly ICurrentBankDetailsRepository _currentBankDetailsRepository;

        public BankTransactionSummaryController(
            IBankTransactionSummaryRepository bankTransactionSummaryRepository, 
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ITransactionFeeRepository transactionFeeRepository,
            ISSDataRepository ssDataRepository,
            ICurrentBankDetailsRepository currentBankDetailsRepository)
        {
            _bankTransactionSummaryRepository = bankTransactionSummaryRepository;
            _transactionRepository = transactionRepository;
            _bankRepository = bankRepository;
            _transactionFeeRepository = transactionFeeRepository;
            _ssDataRepository = ssDataRepository;
            _currentBankDetailsRepository = currentBankDetailsRepository;

        }
        [HttpGet]
        [Route("GetBankTransactionSummary")]
        public async Task<IActionResult> GetBankTransactionSummary(DateTime startDate, DateTime endDate)
        {
            IEnumerable<BankTransactionSummaryViewModel> allFilteredData = await _bankTransactionSummaryRepository.GetBankTransactionSummary(_transactionRepository, _bankRepository, _transactionFeeRepository, _ssDataRepository, _currentBankDetailsRepository, startDate, endDate);
            return Ok(allFilteredData);
        }
        [HttpGet("GetFilteredBankTransactionSummary")]
        public async Task<IActionResult> GetFilteredBankTransactionSummary(DateTime startDate, DateTime endDate, int? bankId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<BankTransactionSummaryViewModel> filteredData;

            // Implemented logic to filter data based on the selected bankId
            if (bankId.HasValue)
            {
                filteredData = await _bankTransactionSummaryRepository.GetFilteredBankTransactionSummary(bankId.Value, _transactionRepository, _bankRepository, _transactionFeeRepository, _ssDataRepository, _currentBankDetailsRepository, startDate, endDate);
                return Ok(filteredData);
            }
            return Ok(Enumerable.Empty<BankTransactionSummaryViewModel>());
        }
        [HttpGet("GetBankChartTransactionSummary")]
        public async Task<IActionResult> GetBankChartTransactionSummary(DateTime startDate, DateTime endDate, string transactionAmountType)
        {
            try
            {
                ChartsViewModel chartsViewModel = await _bankTransactionSummaryRepository.GetBankChartTransactionSummary(startDate, endDate, transactionAmountType, _transactionRepository, _bankRepository, _ssDataRepository);

                if (chartsViewModel != null)
                {
                    return Ok(chartsViewModel);
                }
                else
                {
                    return NotFound("No data found for the specified parameters.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}
