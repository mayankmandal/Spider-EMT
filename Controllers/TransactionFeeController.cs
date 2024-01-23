using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Spider_EMT.Models;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionFeeController : ControllerBase
    {
        private readonly ITransactionFeeRepository _transactionFeeRepository;
        public TransactionFeeController(ITransactionFeeRepository transactionFeeRepository)
        {
            _transactionFeeRepository = transactionFeeRepository;
        }
        [HttpGet]
        [Route("GetTransactionFeeAmount")]
        public async Task<TransactionFee> GetTransactionFeeAmount()
        {
            return await _transactionFeeRepository.GetTransactionFee();
        }

        [HttpPost]
        [Route("AddTransactionFeeAmount")]
        public async Task<IActionResult> AddAllTransactionFeeAmount([FromBody] TransactionFee transactionFee)
        {
            if (transactionFee == null)
            {
                return BadRequest();
            }
            await _transactionFeeRepository.AddTransactionFees(transactionFee);
            return Ok();
        }

        [HttpPut]
        [Route("UpdateTransactionFeeAmount")]
        public async Task<IActionResult> UpdateAllTransactionFeeAmounts([FromBody]TransactionFee transactionFee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _transactionFeeRepository.UpdateTransactionFees(transactionFee);
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

    }
}
