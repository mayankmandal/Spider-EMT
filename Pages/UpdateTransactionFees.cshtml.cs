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
        public TransactionFee? TransactionFeeAmounts { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                // Retrieve the existing transaction fees values
                TransactionFeeAmounts = await _siteSelectionRepository.GetTransactionFee();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Error occurred while loading profile data.");
            }
        }

        public async Task<JsonResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Model State Validation Failed." });
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
                    return new JsonResult(new { success = true, message = TempData["success"] });
                }
                else
                {
                    TempData["error"] = $"Error occurred in response with status: {response.StatusCode} - {response.ReasonPhrase}";
                    return new JsonResult(new { success = false, message = TempData["error"] });
                }
            }
            catch (HttpRequestException ex)
            {
                return HandleError(ex, "Error occurred during HTTP request.");
            }
            catch (JsonException ex)
            {
                return HandleError(ex, "Error occurred while parsing JSON.");
            }
            catch (Exception ex)
            {
                return HandleError(ex, "An unexpected error occurred.");
            }
        }
        private JsonResult HandleError(Exception ex, string errorMessage)
        {
            TempData["error"] = errorMessage + " Error details: " + ex.Message;
            return new JsonResult(new { success = false, message = TempData["error"] });
        }
    }
}
