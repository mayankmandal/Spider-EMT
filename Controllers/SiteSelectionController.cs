using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Domain;
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
        public async Task<IActionResult> GetBankTransactionSummary(DateTime startDate, DateTime endDate)
        {
            IEnumerable<BankTransactionSummaryViewModel> allFilteredData = await _siteSelectionRepository.GetBankTransactionSummary(startDate, endDate);
            return Ok(allFilteredData);
        }
        #endregion

        #region Get Particular Bank's Transaction List for Time Period
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
                filteredData = await _siteSelectionRepository.GetFilteredBankTransactionSummary(bankId.Value, startDate, endDate);
                return Ok(filteredData);
            }
            return Ok(Enumerable.Empty<BankTransactionSummaryViewModel>());
        }
        #endregion

        #region Get Transaction List based on Time Period and Amount Type
        [HttpGet("GetBankChartTransactionSummary")]
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
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
        #endregion

        #region GetAllTransactions
        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime startDate, DateTime endDate)
        {
            return await _siteSelectionRepository.GetAllTransactions(startDate, endDate);
        }
        #endregion

        #region Get All Banks List
        [HttpGet]
        [Route("GetBanks")]
        public async Task<IEnumerable<BankReferenceData>> GetBanks()
        {
            return await _siteSelectionRepository.GetBanks();
        }
        #endregion

        #region Get Current Bank Details
        [HttpGet]
        [Route("GetCurrentBankDetails")]
        public async Task<CurrentBankDetails> GetCurrentBankDetails()
        {
            return await _siteSelectionRepository.GetCurrentBankDetails();
        }
        #endregion

        #region Get Site Selection Data Internally
        [HttpGet]
        [Route("GetSsData")]
        public async Task<IEnumerable<SSDataViewModel>> GetSsData()
        {
            return await _siteSelectionRepository.GetSsData();
        }
        #endregion

        #region Get Terminal Details Based on TerminalId
        [HttpGet]
        [Route("GetTerminalDetails/{terminalId}")]
        public async Task<TerminalDetails> GetTerminalDetails(string terminalId)
        {
            return await _siteSelectionRepository.GetTerminalDetails(terminalId);
        }
        #endregion

        #region Get Per Transaction Fee Amount
        [HttpGet]
        [Route("GetTransactionFeeAmount")]
        public async Task<TransactionFee> GetTransactionFeeAmount()
        {
            return await _siteSelectionRepository.GetTransactionFee();
        }
        #endregion

        #region Add New Per Transaction Fee Amount
        [HttpPost]
        [Route("AddTransactionFeeAmount")]
        public async Task<IActionResult> AddAllTransactionFeeAmount([FromBody] TransactionFee transactionFee)
        {
            if (transactionFee == null)
            {
                return BadRequest();
            }
            await _siteSelectionRepository.AddTransactionFees(transactionFee);
            return Ok();
        }
        #endregion

        #region Update Existing Per Transaction Fee Amount
        [HttpPut]
        [Route("UpdateTransactionFeeAmount")]
        public async Task<IActionResult> UpdateAllTransactionFeeAmounts([FromBody] TransactionFee transactionFee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _siteSelectionRepository.UpdateTransactionFees(transactionFee);
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(500, $"SQL Exception: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
            return NoContent();
        }
        #endregion

        #endregion
    }
}
