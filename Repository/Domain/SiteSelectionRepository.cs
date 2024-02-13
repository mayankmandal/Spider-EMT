using AutoMapper;
using DecryptDll;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class SiteSelectionRepository : ISiteSelectionRepository
    {
        private const string BankLogoFolderPath = "/images/bank_logos/";
        private readonly string _ssDataFilePath;
        private readonly IMapper _mapper;

        public SiteSelectionRepository(string ssDataFilePath, IMapper mapper)
        {
            _ssDataFilePath = ssDataFilePath;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                string commandText = "SELECT [TxnId],[TermId],[TxnDate],[TotalCWCount],[TotalBICount],[TotalMSCount],[TotalCWAmount] " +
                         "FROM [SpiderETMDB].[dbo].[tblAllAtmTxn] " +
                         "WHERE [TxnDate] >= @StartDate AND [TxnDate] <= @EndDate";

                SqlParameter[] parameters =
                {
                new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = StartDate },
                new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = EndDate }
            };

                DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(commandText, CommandType.Text, parameters);

                List<AtmTransactionData> transactions = new List<AtmTransactionData>();

                foreach (DataRow row in dataTable.Rows)
                {
                    AtmTransactionData transaction = MapDataRowToModel(row);
                    transactions.Add(transaction);
                }

                return transactions;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in GetAllTransactions.", ex);
            }
        }
        private static AtmTransactionData MapDataRowToModel(DataRow row)
        {
            try
            {
                return new AtmTransactionData
                {
                    TxnId = (long)row["TxnId"],
                    TermId = row["TermId"].ToString(),
                    TxnDate = (DateTime)row["TxnDate"],
                    TotalCWCount = (int)row["TotalCWCount"],
                    TotalBICount = (int)row["TotalBICount"],
                    TotalMSCount = (int)row["TotalMSCount"],
                    TotalCWAmount = (decimal)row["TotalCWAmount"]
                };
            }
            catch (Exception ex)
            {
                // Log or handle exceptions during mapping
                throw new Exception("Error mapping DataRow to AtmTransactionData.", ex);
            }
        }
        public async Task<IEnumerable<BankReferenceData>> GetBanks()
        {
            try
            {
                string commandText = "SELECT * FROM tblRefBanks";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<BankReferenceData> banks = new List<BankReferenceData>();

                foreach (DataRow row in dataTable.Rows)
                {
                    BankReferenceData bank = new BankReferenceData
                    {
                        BankId = (int)row["BankId"],
                        BankName = row["BankName"].ToString()
                    };
                    banks.Add(bank);
                }

                return banks;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error executing SQL command in GetBanks.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBanks.", ex);
            }
        }
        public async Task<IEnumerable<BankTransactionSummaryViewModel>> GetBankTransactionSummary(DateTime StartDate,
                DateTime EndDate)
        {
            try
            {
                // Retrieve data from the required tables
                var atmData = await GetAllTransactions(StartDate, EndDate);
                var bankData = await GetBanks();
                var txnFee = await GetTransactionFee();
                var ssData = await GetSsData();
                var currentbankdetailsData = await GetCurrentBankDetails();

                // Business logic to calculate TotalCWFeeAmount, TotalBI_MSFeeAmount, TotalTxnOnUsCount, TotalPayedAmount
                var result = from atm in atmData
                             join ss in ssData on atm.TermId equals ss.TermId
                             join bank in bankData on ss.BankNameEn equals bank.BankName
                             where (atm.TxnDate >= StartDate && atm.TxnDate <= EndDate)
                             select new BankTransactionSummaryViewModel
                             {
                                 BankNameEn = ss.BankNameEn,
                                 // Assumed logos are in PNG format
                                 BankLogoPath = $"{BankLogoFolderPath}{ss.BankShortName}" + ".png",
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
            catch (SqlException sqlEx)
            {
                throw new Exception("Error executing SQL command in GetBankTransactionSummary.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBankTransactionSummary.", ex);
            }
        }

        public async Task<IEnumerable<BankTransactionSummaryViewModel>> GetFilteredBankTransactionSummary(int bankId,
            DateTime StartDate,
            DateTime EndDate)
        {
            try
            {
                // Retrieve data from the required tables
                var atmData = await GetAllTransactions(StartDate, EndDate);
                var bankData = await GetBanks();
                var txnFee = await GetTransactionFee();
                var ssData = await GetSsData();
                var currentbankdetailsData = await GetCurrentBankDetails();

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
            catch (SqlException sqlEx)
            {
                throw new Exception("Error executing SQL command in GetFilteredBankTransactionSummary.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetFilteredBankTransactionSummary.", ex);
            }
        }

        public async Task<ChartsViewModel> GetBankChartTransactionSummary(DateTime startDate,
            DateTime endDate,
            string transactionAmountType,
            ISiteSelectionRepository siteSelectionRepository)
        {
            try
            {
                ChartsViewModel chartsViewModel = new ChartsViewModel();

                // Retrieve data from the required tables
                var atmData = await siteSelectionRepository.GetAllTransactions(startDate, endDate);
                var bankData = await siteSelectionRepository.GetBanks();
                var ssData = await siteSelectionRepository.GetSsData();

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
                /*chartsViewModel.ChartTransactionDataList = result.ToList();*/
                chartsViewModel.FromDate = startDate;
                chartsViewModel.ToDate = endDate;
                chartsViewModel.TransactionAmountType = transactionAmountType;
                return chartsViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBankChartTransactionSummary.", ex);
            }
        }
        public async Task<CurrentBankDetails> GetCurrentBankDetails()
        {
            try
            {
                string commandText = "SELECT * FROM tblCurrentBankDetails";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                CurrentBankDetails currentbankdetails = new CurrentBankDetails();

                foreach (DataRow row in dataTable.Rows)
                {
                    currentbankdetails = new CurrentBankDetails
                    {
                        CurrentBankId = (int)row["CurrentBankId"],
                        CurrentBankName = row["CurrentBankName"].ToString(),
                        CurrentBankLogoPath = $"{BankLogoFolderPath}{row["CurrentBankShortName"]}" + ".png"
                    };
                }
                return currentbankdetails;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error executing SQL command in GetCurrentBankDetails.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCurrentBankDetails.", ex);
            }
        }
        public async Task<IEnumerable<SSDataViewModel>> GetSsData()
        {
            try
            {
                var ssData = GetAllSsData();
                return ssData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetSsData.", ex);
            }
        }
        private IEnumerable<SSDataViewModel> GetAllSsData()
        {
            try
            {
                string encryptedJson = File.ReadAllText(_ssDataFilePath);

                DecryptText decrypt = new DecryptText();
                string decryptedJson = decrypt.Decrypt(encryptedJson);

                // Deserialize decrypted JSON string into SSDataViewModel
                IEnumerable<SSDataViewModel> AllSsData = JsonConvert.DeserializeObject<List<SSDataViewModel>>(decryptedJson);
                return AllSsData;
            }
            catch (FileNotFoundException ex)
            {
                // Handle the case where the file is not found
                throw new Exception("Error in GetAllSsData. File not found.", ex);
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exceptions
                throw new Exception("Error in GetAllSsData. JSON deserialization failed.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetAllSsData.", ex);
            }
        }
        public async Task<TransactionFee> GetTransactionFee()
        {
            try
            {
                string commandText = "SELECT * FROM tblRefTxnFee";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                TransactionFee transactionFee = new TransactionFee();

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    transactionFee = new TransactionFee
                    {
                        TxnFeeId = (int)row["TxnFeeId"],
                        CWTxnFee = (decimal)row["CWTxnFee"],
                        BITxnFee = (decimal)row["BITxnFee"],
                        MSTxnFee = (decimal)row["MSTxnFee"]
                    };
                }

                return transactionFee;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error executing SQL command in GetTransactionFee.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTransactionFee.", ex);
            }

        }

        public async Task<bool> UpdateTransactionFees(TransactionFee transactionFee)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@TransactionFeeId", SqlDbType.Int) { Value = transactionFee.TxnFeeId },
                    new SqlParameter("@NewCWTxnFee", SqlDbType.Decimal) { Value = transactionFee.CWTxnFee },
                    new SqlParameter("@NewBITxnFee", SqlDbType.Decimal) { Value = transactionFee.BITxnFee },
                    new SqlParameter("@NewMSTxnFee", SqlDbType.Decimal) { Value = transactionFee.MSTxnFee }
                };

                bool result = await Task.Run(() =>
                {
                    var procedureName = Constants.SP_UpdateTransactionFees;
                    return SqlDBHelper.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, parameters);
                });

                return result;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error updating transaction fees - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating transaction fees.", ex);
            }
        }
        public async Task<bool> AddTransactionFees(TransactionFee transactionFee)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@NewCWTxnFee",SqlDbType.Decimal){Value = transactionFee.CWTxnFee},
                    new SqlParameter("@NewBITxnFee",SqlDbType.Decimal){Value = transactionFee.BITxnFee},
                    new SqlParameter("@NewMSTxnFee",SqlDbType.Decimal){Value = transactionFee.MSTxnFee}
                };
                bool result = await Task.Run(() =>
                {
                    var procedureName = Constants.SP_AddTransactionFees;
                    return SqlDBHelper.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, parameters);
                });
                return result;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding transaction fees - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding transaction fees.", ex);
            }
        }
        public async Task<TerminalDetails> GetTerminalDetails(string terminalId)
        {
            try
            {
                var AllSsData = GetSsData();
                var termSSData = AllSsData.Result.FirstOrDefault(ss => ss.TermId == terminalId);
                var terminalDetailsData = _mapper.Map<TerminalDetails>(termSSData);
                return terminalDetailsData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTerminalDetails.", ex);
            }
        }
    }
}
