using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class BankRepository: IBankRepository
    {
        public async Task<IEnumerable<BankReferenceData>> GetBanks()
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
    }
}
