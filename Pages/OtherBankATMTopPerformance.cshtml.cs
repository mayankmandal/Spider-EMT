using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Pages
{
    public class OtherBankATMTopPerformanceModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public OtherBankATMTopPerformanceModel(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        public IEnumerable<BankReferenceData> Banks { get; set; }
        public TransactionFee TransactionFeesAmount { get; set; }
        public IEnumerable<BankTransactionSummaryViewModel> BankTransactionSummary { get; set; }
        public CurrentBankDetails CurrentBankDetailsData { get; set; }

        public async Task OnGet()
        {
            await LoadBanks();
            await LoadTransactionFees();
            await LoadCurrentBankDetails();
        }

        private async Task LoadBanks()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/SiteSelection/GetBanks");
            Banks = JsonConvert.DeserializeObject<IEnumerable<BankReferenceData>>(response);
        }

        private async Task LoadTransactionFees()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/SiteSelection/GetTransactionFeeAmount");
            TransactionFeesAmount = JsonConvert.DeserializeObject<TransactionFee>(response);
        }
        private async Task LoadCurrentBankDetails()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/SiteSelection/GetCurrentBankDetails");
            CurrentBankDetailsData = JsonConvert.DeserializeObject<CurrentBankDetails>(response);
        }
    }
}
