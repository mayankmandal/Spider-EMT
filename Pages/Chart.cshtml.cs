using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Pages
{
    public class ChartModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public ChartModel(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = clientFactory;
        }
        [BindProperty]
        public ChartsViewModel ChartsViewModelData { get; set; }

        public IActionResult OnGet()
        {
            if (ChartsViewModelData == null)
            {
                ChartsViewModelData = new ChartsViewModel()
                {
                    // Set FromDate to 90 days ago from the current date
                    FromDate = DateTime.Now.AddDays(-90),
                    ToDate = DateTime.Now,
                    TransactionAmountType = "CW",
                    ChartTransactionDataList = new List<ChartTransactionData>()
                };
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = $"{_configuration["ApiBaseUrl"]}/SiteSelection/GetBankChartTransactionSummary?" +
                             $"transactionAmountType={Uri.EscapeUriString(ChartsViewModelData.TransactionAmountType)}" +
                             $"&startDate={Uri.EscapeDataString(ChartsViewModelData.FromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}" +
                             $"&endDate={Uri.EscapeDataString(ChartsViewModelData.ToDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}";
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ChartsViewModelData = JsonConvert.DeserializeObject<ChartsViewModel>(responseBody);
                    return Page();
                }
                else
                {
                    TempData["error"] = "Error occured in response with status : " + response.StatusCode;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error Occured : " + ex.Message;
                return StatusCode(500, $"Internal Server Error : {ex.Message}");
            }
        }
    }
}
