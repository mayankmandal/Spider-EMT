using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CurrentUser _currentUser;
        public ErrorLogRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _currentUser = new CurrentUser
            {
                UserId = Int32.Parse(_configuration["RecentUserId"])
            };
        }

        public async Task<int> LogErrorAsync(ErrorLog errorLog)
        {
            int LogId = -1;
            // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewErrorMessage", SqlDbType.VarChar, int.MaxValue) { Value = errorLog.ErrorMessage },
                    new SqlParameter("@NewStackTrace", SqlDbType.VarChar, int.MaxValue) { Value = errorLog.StackTrace },
                    new SqlParameter("@NewCreateUserId", SqlDbType.Int) { Value = _currentUser.UserId },
                    new SqlParameter("@NewUpdateUserId", SqlDbType.Int) { Value = _currentUser.UserId }
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(Constants.SP_InsertNewErrorLog, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        LogId = (int)dataRow["LogId"];
                    }
                    if (LogId <= 0)
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            return LogId;
        }
    }
}
