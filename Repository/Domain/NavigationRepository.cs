using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class NavigationRepository : INavigationRepository
    {
        public NavigationRepository()
        {

        }

        public List<Profile> GetAllProfiles()
        {
            try
            {
                string commandText = "SELECT ProfileId, ProfileName FROM tblProfile";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<Profile> profiles = new List<Profile>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Profile profile = new Profile
                        {
                            ProfileId = (int)row["ProfileId"],
                            ProfileName = row["ProfileName"].ToString()
                        };
                        profiles.Add(profile);
                    }
                }
                return profiles;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in GetAllProfiles.", ex);
            }
        }

        public List<Page> GetAllPages()
        {
            try
            {
                string commandText = "SELECT PageId, PageUrl, PageDescription, MenuImgPath FROM tblPage";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<Page> pages = new List<Page>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Page page = new Page
                        {
                            PageId = (int)row["PageId"],
                            PageUrl = row["PageUrl"].ToString(),
                            PageDescription = row["PageDescription"].ToString(),
                            MenuImgPath = row["MenuImgPath"].ToString()
                        };
                        pages.Add(page);
                    }
                }
                return pages;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in tblPage.", ex);
            }
        }

        public List<PageCategory> GetAllPageCategories()
        {
            try
            {
                string commandText = "SELECT PageCatId, CatagoryName FROM tblPageCatagory";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageCategory> pageCategories = new List<PageCategory>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageCategory pageCategory = new PageCategory
                        {
                            PageCatId = (int)row["PageCatId"],
                            CategoryName = row["CatagoryName"].ToString()
                        };
                        pageCategories.Add(pageCategory);
                    }
                }
                return pageCategories;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in GetAllPageCategories.", ex);
            }
        }

        public List<CurrentUserProfileViewModel> GetCurrentProfiles()
        {
            try
            {
                string commandText = "SELECT pro.ProfileId,ProfileName,usrpro.UserId from tblProfile pro INNER JOIN tblUserProfile usrpro on pro.ProfileId = usrpro.ProfileId INNER JOIN tblCurrentUser cur on usrpro.UserId = cur.UserId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<CurrentUserProfileViewModel> userProfiles = new List<CurrentUserProfileViewModel>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CurrentUserProfileViewModel userProfile = new CurrentUserProfileViewModel
                        {
                            ProfileId = (int)row["ProfileId"],
                            ProfileName = row["ProfileName"].ToString(),
                            UserId = (int)row["UserId"],
                        };
                        userProfiles.Add(userProfile);
                    }
                }
                return userProfiles;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in GetAllPageCategories.", ex);
            }
        }

        /*public async Task<TransactionFee> GetUserProfile()
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

        public async Task<bool> UpdateUserProfile(TransactionFee transactionFee)
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
        }*/

        public async Task<bool> AddUserProfile(Profile profile)
        {
            try
            {
                string commandText = "INSERT INTO tblProfile (ProfileName) VALUES (@ProfileName)";

                // Parameters
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileName", SqlDbType.VarChar, 50) { Value = profile.ProfileName }
                };

                // Execute the command
                bool isSuccess = SqlDBHelper.ExecuteNonQuery(commandText, CommandType.Text, sqlParameters);

                return isSuccess;

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

        public async Task<bool> AddUserPermissions(UserPermission userPermission)
        {
            try
            {
                string commandText = "INSERT INTO tblUserPermission (ProfileId, PageId) VALUES (@ProfileId, @PageId))";

                // Parameters
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.VarChar, 50) { Value = userPermission.ProfileId },
                    new SqlParameter("@PageId", SqlDbType.Int) { Value = userPermission.PageId }
                };

                // Execute the command
                bool isSuccess = SqlDBHelper.ExecuteNonQuery(commandText, CommandType.Text, sqlParameters);

                return isSuccess;

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
    }
}
