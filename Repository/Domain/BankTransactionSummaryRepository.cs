using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Repository.Domain
{
    public class BankTransactionSummaryRepository : IBankTransactionSummaryRepository
    {
        private const string BankLogoFolderPath = "/images/bank_logos/";
        public async Task<IEnumerable<BankTransactionSummaryViewModel>> GetBankTransactionSummary(IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ITransactionFeeRepository transactionFeeRepository,
            ISSDataRepository ssDataRepository,
            ICurrentBankDetailsRepository currentBankDetailsRepository,
                DateTime StartDate, 
                DateTime EndDate)
        {        
            // Retrieve data from the required tables
            var atmData = await transactionRepository.GetAllTransactions(StartDate, EndDate);
            var bankData = await bankRepository.GetBanks();
            var txnFee = await transactionFeeRepository.GetTransactionFee();
            var ssData = await ssDataRepository.GetSsData();
            var currentbankdetailsData = await currentBankDetailsRepository.GetCurrentBankDetails();

            // Business logic to calculate TotalCWFeeAmount, TotalBI_MSFeeAmount, TotalTxnOnUsCount, TotalPayedAmount
            var result = from atm in atmData
                         join ss in ssData on atm.TermId equals ss.TermId
                         join bank in bankData on ss.BankNameEn equals bank.BankName
                         where (atm.TxnDate >= StartDate && atm.TxnDate <= EndDate)
                         select new BankTransactionSummaryViewModel
                         {
                             BankNameEn = ss.BankNameEn,
                             // Assumed logos are in PNG format
                             BankLogoPath = $"{BankLogoFolderPath}{ss.BankShortName}"+ ".png", 
                             TermId = atm.TermId,
                             RegionEn = ss.RegionEn,
                             CityEn = ss.CityEn,
                             TxnDate = atm.TxnDate,
                             TotalCWCount = atm.TotalCWCount,
                             TotalCWFeeAmount = atm.TotalCWCount * txnFee.CWTxnFee,
                             TotalBICount = atm.TotalBICount,
                             TotalMSCount = atm.TotalMSCount,
                             TotalBI_MSFeeAmount = (atm.TotalBICount * txnFee.BITxnFee) + (atm.TotalMSCount * txnFee.MSTxnFee),
                             TotalTxnOnUsCount = atm.TotalCWCount + atm.TotalBICount + atm.TotalMSCount,
                             TotalPayedAmount = (atm.TotalCWCount * txnFee.CWTxnFee) + ((atm.TotalBICount * txnFee.BITxnFee) + (atm.TotalMSCount * txnFee.MSTxnFee))
                         };
            var modifiedResult = result.ToList();
            // Business logic to show calculated TotalCWFeeAmount, TotalBI_MSFeeAmount, TotalPayedAmount as 0 for same banks
            foreach (var item in modifiedResult.Where(x => x.BankNameEn == currentbankdetailsData.CurrentBankName))
            {
                item.TotalCWFeeAmount = 0;
                item.TotalBI_MSFeeAmount = 0;
                item.TotalPayedAmount = 0;
            }
            return modifiedResult;
        }

        public async Task<IEnumerable<BankTransactionSummaryViewModel>> GetFilteredBankTransactionSummary(int bankId,
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ITransactionFeeRepository transactionFeeRepository,
            ISSDataRepository ssDataRepository,
            ICurrentBankDetailsRepository currentBankDetailsRepository,
            DateTime StartDate,
            DateTime EndDate)
        {
            // Retrieve data from the required tables
            var atmData = await transactionRepository.GetAllTransactions(StartDate, EndDate);
            var bankData = await bankRepository.GetBanks();
            var txnFee = await transactionFeeRepository.GetTransactionFee();
            var ssData = await ssDataRepository.GetSsData();
            var currentbankdetailsData = await currentBankDetailsRepository.GetCurrentBankDetails();

            // Business logic to calculate TotalCWFeeAmount, TotalBI_MSFeeAmount, TotalTxnOnUsCount, TotalPayedAmount
            var result = from atm in atmData
                         join ss in ssData on atm.TermId equals ss.TermId
                         join bank in bankData on ss.BankNameEn equals bank.BankName
                         where bank.BankId == bankId // Added this condition to filter by BankId
                         && (atm.TxnDate >= StartDate && atm.TxnDate <= EndDate)
                         select new BankTransactionSummaryViewModel
                         {
                             BankNameEn = ss.BankNameEn,
                             // Assumed logos are in PNG format
                             BankLogoPath = $"{BankLogoFolderPath}{ss.BankShortName}.png",
                             TermId = atm.TermId,
                             RegionEn = ss.RegionEn,
                             CityEn = ss.CityEn,
                             TxnDate = atm.TxnDate,
                             TotalCWCount = atm.TotalCWCount,
                             TotalCWFeeAmount = atm.TotalCWCount * txnFee.CWTxnFee,
                             TotalBICount = atm.TotalBICount,
                             TotalMSCount = atm.TotalMSCount,
                             TotalBI_MSFeeAmount = (atm.TotalBICount * txnFee.BITxnFee) + (atm.TotalMSCount * txnFee.MSTxnFee),
                             TotalTxnOnUsCount = atm.TotalCWCount + atm.TotalBICount + atm.TotalMSCount,
                             TotalPayedAmount = (atm.TotalCWCount * txnFee.CWTxnFee) + ((atm.TotalBICount * txnFee.BITxnFee) + (atm.TotalMSCount * txnFee.MSTxnFee))
                         };

            var modifiedResult = result.ToList();
            // Business logic to show calculated TotalCWFeeAmount, TotalBI_MSFeeAmount, TotalPayedAmount as 0 for same banks
            foreach (var item in modifiedResult.Where(x => x.BankNameEn == currentbankdetailsData.CurrentBankName))
            {
                item.TotalCWFeeAmount = 0;
                item.TotalBI_MSFeeAmount = 0;
                item.TotalPayedAmount = 0;
            }
            return modifiedResult;
        }

        public async Task<ChartsViewModel> GetBankChartTransactionSummary(DateTime startDate, 
            DateTime endDate, 
            string transactionAmountType, 
            IAtmTransactionRepository transactionRepository,
            IBankRepository bankRepository,
            ISSDataRepository ssDataRepository)
        {
            ChartsViewModel chartsViewModel = new ChartsViewModel();

            // Retrieve data from the required tables
            var atmData = await transactionRepository.GetAllTransactions(startDate, endDate);
            var bankData = await bankRepository.GetBanks();
            var ssData = await ssDataRepository.GetSsData();

            // Business logic to fetch BankName and AverageAmount
            var result = from bank in bankData
                         join ss in ssData on bank.BankName equals ss.BankNameEn into ssGroup
                         from ss in ssGroup.DefaultIfEmpty()
                         join atm in atmData
                             .Where(atm => atm.TxnDate >= startDate && atm.TxnDate <= endDate)
                             on ss?.TermId equals atm.TermId into atmGroup
                         from atm in atmGroup.DefaultIfEmpty()
                         group atm by new { bank.BankName, ss.BankShortName } into grouped
                         select new ChartTransactionData
                         {
                             BankNameEn = grouped.Key.BankName,
                             BankShortName = grouped.Key.BankShortName,
                             AverageAmount = transactionAmountType switch
                             {
                                 "CW" => Convert.ToDecimal(grouped.Sum(item => item?.TotalCWCount ?? 0)),
                                 "MS" => Convert.ToDecimal(grouped.Sum(item => item?.TotalMSCount ?? 0)),
                                 "BI" => Convert.ToDecimal(grouped.Sum(item => item?.TotalBICount ?? 0)),
                                 _ => 0 // Default value if transactionAmountType is not recognized
                             }
                         };
            chartsViewModel.ChartTransactionDataList = result.Where(data => data.BankShortName != null && !data.BankShortName.Contains("\r\n")).ToList();
            chartsViewModel.FromDate = startDate;
            chartsViewModel.ToDate = endDate;
            chartsViewModel.TransactionAmountType = transactionAmountType;
            return chartsViewModel;
        }
    }
}
