using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class CurrentBankDetailsRepository : ICurrentBankDetailsRepository
    {
        public async Task<CurrentBankDetails> GetCurrentBankDetails()
        {
            string commandText = "SELECT * FROM tblCurrentBankDetails";
            DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

            CurrentBankDetails currentbankdetails = new CurrentBankDetails();

            foreach (DataRow row in dataTable.Rows)
            {
                currentbankdetails = new CurrentBankDetails
                {
                    CurrentBankId = (int)row["CurrentBankId"],
                    CurrentBankName = row["CurrentBankName"].ToString()
                };
            }
            return currentbankdetails;
        }
    }
}
