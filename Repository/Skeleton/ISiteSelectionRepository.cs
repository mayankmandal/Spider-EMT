using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface ISiteSelectionRepository
    {
        Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime StartDate, DateTime EndDate);
        Task<IEnumerable<BankReferenceData>> GetBanks();
        Task<IEnumerable<BankTransactionSummaryViewModel>> GetBankTransactionSummary(
            DateTime StartDate,
            DateTime EndDate);
        Task<IEnumerable<BankTransactionSummaryViewModel>> GetFilteredBankTransactionSummary(int BankId,
            DateTime StartDate,
            DateTime EndDate);
        Task<ChartsViewModel> GetBankChartTransactionSummary(DateTime startDate,
            DateTime endDate,
            string transactionAmountType,
            ISiteSelectionRepository siteSelectionRepository);
        Task<CurrentBankDetails> GetCurrentBankDetails();
        Task<IEnumerable<SSDataViewModel>> GetSsData();
        Task<TerminalDetails> GetTerminalDetails(string terminalId);
        Task<TransactionFee> GetTransactionFee();
        Task<bool> UpdateTransactionFees(TransactionFee transactionFee);
        Task<bool> AddTransactionFees(TransactionFee transactionFee);
    }
}
