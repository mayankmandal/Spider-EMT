using Spider_EMT.Models.ViewModels;

namespace Spider_EMT.Repository.Skeleton
{
    public interface IBankTransactionSummaryRepository
    {
        Task<IEnumerable<BankTransactionSummaryViewModel>> GetBankTransactionSummary(
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ITransactionFeeRepository transactionFeeRepository,
            ISSDataRepository ssDataRepository,
            ICurrentBankDetailsRepository currentBankDetailsRepository,
            DateTime StartDate,
            DateTime EndDate);
        Task<IEnumerable<BankTransactionSummaryViewModel>> GetFilteredBankTransactionSummary(int BankId,
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ITransactionFeeRepository transactionFeeRepository,
            ISSDataRepository ssDataRepository,
            ICurrentBankDetailsRepository currentBankDetailsRepository,
            DateTime StartDate,
            DateTime EndDate);
        Task<ChartsViewModel> GetBankChartTransactionSummary(DateTime startDate, 
            DateTime endDate, 
            string transactionAmountType,
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ISSDataRepository ssDataRepository);
    }
}
