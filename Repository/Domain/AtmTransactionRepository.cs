using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class AtmTransactionRepository : IAtmTransactionRepository
    {
        public async Task<IEnumerable<AtmTransactionData>> GetAllTransactions(DateTime StartDate, DateTime EndDate)
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
        private static AtmTransactionData MapDataRowToModel(DataRow row)
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
    }
}
