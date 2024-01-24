using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Text;

namespace Spider_EMT.Pages
{
    public class UpdateTransactionFeesModel : PageModel
    {
        private readonly ISiteSelectionRepository _siteSelectionRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public UpdateTransactionFeesModel(ISiteSelectionRepository siteSelectionRepository, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _siteSelectionRepository = siteSelectionRepository;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        [BindProperty]
        public TransactionFee TransactionFeeAmounts { get; set; }
        public async Task<IActionResult> OnGet()
        {
            // Retrieve the existing transaction fees values
            TransactionFeeAmounts = await _siteSelectionRepository.GetTransactionFee();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/SiteSelection/UpdateTransactionFeeAmount";
                var jsonContent = JsonConvert.SerializeObject(TransactionFeeAmounts);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (TransactionFeeAmounts.TxnFeeId > 0)
                {
                    response = await client.PutAsync(apiUrl, httpContent);
                }
                else
                {
                    // API endpoint for adding new data
                    apiUrl = $"{_configuration["ApiBaseUrl"]}/SiteSelection/AddTransactionFeeAmount";
                    response = await client.PostAsync(apiUrl, httpContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    if (TransactionFeeAmounts.TxnFeeId > 0)
                    {
                        TempData["success"] = "Transaction Fee Amounts Updated Successfully";
                    }
                    else
                    {
                        TempData["success"] = "Transaction Fee Amounts Added Successfully";
                    }
                    // Redirect back to the same page
                    return RedirectToPage();
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error occured : " + ex.Message;
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
