using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Data;
using Spider_EMT.Utility;

namespace Spider_EMT.Repository.Domain
{
    public class TransactionFeeRepository : ITransactionFeeRepository
    {
        public async Task<TransactionFee> GetTransactionFee()
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
            catch(SqlException sqlEx)
            {
                throw new Exception("Error updating transaction fees - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating transaction fees.", ex);
            }
        }
    }
}
