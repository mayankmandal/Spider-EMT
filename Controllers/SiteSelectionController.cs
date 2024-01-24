using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteSelectionController : ControllerBase
    {
        #region Fields
        private readonly ISiteSelectionRepository _siteSelectionRepository;
        #endregion

        #region Constructor
        public SiteSelectionController(ISiteSelectionRepository siteSelectionRepository)
        {
            _siteSelectionRepository = siteSelectionRepository;
        }
        #endregion

        #region Actions

        #region Get All the Bank Transaction List for Time Period
        [HttpGet]
        [Route("GetBankTransactionSummary")]
        [ProducesResponseType(typeof(IEnumerable<BankTransactionSummaryViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBankTransactionSummary(DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<BankTransactionSummaryViewModel> allFilteredData = await _siteSelectionRepository.GetBankTransactionSummary(startDate, endDate);
                return Ok(allFilteredData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }
        #endregion

        #region Get Particular Bank's Transaction List for Time Period
        [HttpGet("GetFilteredBankTransactionSummary")]
        [ProducesResponseType(typeof(IEnumerable<BankTransactionSummaryViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredBankTransactionSummary(DateTime startDate, DateTime endDate, int? bankId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IEnumerable<BankTransactionSummaryViewModel> filteredData;

                // Implemented logic to filter data based on the selected bankId
                if (bankId.HasValue)
                {
                    filteredData = await _siteSelectionRepository.GetFilteredBankTransactionSummary(bankId.Value, startDate, endDate);
                    return Ok(filteredData);
                }
                return Ok(Enumerable.Empty<BankTransactionSummaryViewModel>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get Transaction List based on Time Period and Amount Type for Chart
        [HttpGet("GetBankChartTransactionSummary")]
        [ProducesResponseType(typeof(ChartsViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBankChartTransactionSummary(DateTime startDate, DateTime endDate, string transactionAmountType)
        {
            try
            {
                ChartsViewModel chartsViewModel = await _siteSelectionRepository.GetBankChartTransactionSummary(startDate, endDate, transactionAmountType, _siteSelectionRepository);

                if (chartsViewModel != null)
                {
                    return Ok(chartsViewModel);
                }
                else
                {
                    return NotFound("No data found for the specified parameters.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region GetAllTransactions
        [HttpGet]
        [Route("GetAllTransactions")]
        [ProducesResponseType(typeof(IEnumerable<AtmTransactionData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTransactions(DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<AtmTransactionData> allTransactions = await _siteSelectionRepository.GetAllTransactions(startDate, endDate);
                return Ok(allTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get All Banks List
        [HttpGet]
        [Route("GetBanks")]
        [ProducesResponseType(typeof(IEnumerable<BankReferenceData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                IEnumerable<BankReferenceData> banks = await _siteSelectionRepository.GetBanks();
                return Ok(banks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get Current Bank Details
        [HttpGet]
        [Route("GetCurrentBankDetails")]
        [ProducesResponseType(typeof(CurrentBankDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentBankDetails()
        {
            try
            {
                CurrentBankDetails currentBankDetails = await _siteSelectionRepository.GetCurrentBankDetails();
                return Ok(currentBankDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get Site Selection Data Internally
        [HttpGet]
        [Route("GetSsData")]
        [ProducesResponseType(typeof(IEnumerable<SSDataViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSsData()
        {
            try
            {
                IEnumerable<SSDataViewModel> ssData = await _siteSelectionRepository.GetSsData();
                return Ok(ssData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get Terminal Details Based on TerminalId
        [HttpGet]
        [Route("GetTerminalDetails/{terminalId}")]
        [ProducesResponseType(typeof(TerminalDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTerminalDetails(string terminalId)
        {
            try
            {
                TerminalDetails terminalDetails = await _siteSelectionRepository.GetTerminalDetails(terminalId);
                return Ok(terminalDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Get Per Transaction Fee Amount
        [HttpGet]
        [Route("GetTransactionFeeAmount")]
        [ProducesResponseType(typeof(TransactionFee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactionFeeAmount()
        {
            try
            {
                TransactionFee transactionFee = await _siteSelectionRepository.GetTransactionFee();
                return Ok(transactionFee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Add New Per Transaction Fee Amount
        [HttpPost]
        [Route("AddTransactionFeeAmount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAllTransactionFeeAmount([FromBody] TransactionFee transactionFee)
        {
            try
            {
                if (transactionFee == null)
                {
                    return BadRequest();
                }
                await _siteSelectionRepository.AddTransactionFees(transactionFee);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #region Update Existing Per Transaction Fee Amount
        [HttpPut]
        [Route("UpdateTransactionFeeAmount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAllTransactionFeeAmounts([FromBody] TransactionFee transactionFee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _siteSelectionRepository.UpdateTransactionFees(transactionFee);
                return NoContent();
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, $"SQL Exception: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        #endregion

        #endregion
    }
}
