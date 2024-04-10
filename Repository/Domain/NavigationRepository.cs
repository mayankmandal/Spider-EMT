using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Data;

namespace Spider_EMT.Repository.Domain
{
    public class NavigationRepository : INavigationRepository
    {
        public NavigationRepository()
        {

        }

        public List<ProfileSite> GetAllProfiles()
        {
            try
            {
                string commandText = "SELECT ProfileId, ProfileName FROM tblProfile";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<ProfileSite> profiles = new List<ProfileSite>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        ProfileSite profile = new ProfileSite
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

        public List<PageSite> GetAllPages()
        {
            try
            {
                string commandText = "SELECT PageId, PageUrl, PageDescription, MenuImgPath FROM tblPage";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageSite> pages = new List<PageSite>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageSite page = new PageSite
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
                            CategoryName = row["CatagoryName"].ToString(),
                            PageId = 0
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
        public List<PageCategory> GetPageToCategories(List<int> pageList) 
        {
            try
            {
                List<PageCategory> pageCategories = new List<PageCategory>();

                foreach(int pageId in pageList)
                {
                    string commandText = "SELECT DISTINCT tpc.PageCatId, tpc.CatagoryName from tblPageCatagory tpc INNER JOIN tblPage tp on tpc.PageCatId = tp.PageCatId WHERE tp.PageId = @PageId";
                    SqlParameter[] sqlParameter =
                    {
                        new SqlParameter("@PageId", SqlDbType.Int){Value = pageId}
                    };
                    DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(commandText, CommandType.Text, sqlParameter);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            PageCategory pageCategory = new PageCategory
                            {
                                CategoryName = row["CatagoryName"].ToString(),
                                PageCatId = (int)row["PageCatId"],
                                PageId = pageId
                            };
                            // Check if the page category already exists in the list
                            if (!pageCategories.Any(pc => pc.PageCatId == pageCategory.PageCatId))
                            {
                                pageCategories.Add(pageCategory);
                            }
                        }
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

        public async Task<bool> AddUserProfile(ProfileSite profile, List<PageSite> pages, List<PageCategory> pageCategories)
        {
            try
            {
                int ProfileId = 0;

                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewProfileName", SqlDbType.VarChar, 50) { Value = profile.ProfileName }
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(Constants.SP_AddUserProfile, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        ProfileId = (int)dataRow["ProfileId"];
                    }
                    else
                    {
                        return false;
                    }
                }

                // User Permission Allotment based on categories list provided already having associated pages linked
                foreach (PageCategory pageCategory in pageCategories)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageCategory.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = pageCategory.PageCatId}
                    };

                    bool isSuccess = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (!isSuccess)
                    {
                        return false;
                    }
                    
                }

                // Filter out pages with matching PageCatId
                var filteredPages = pages.Where(p => !pageCategories.Any(pc => pc.PageCatId == p.PageCatId)).ToList();

                // User Permission Allotment based on pages list provided, removed already inserted categories asscoaited pages
                foreach (PageSite pagesite in filteredPages)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pagesite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = pagesite.PageCatId}
                    };

                    bool isSuccess = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding transaction fees - SQL Exception.", sqlEx);
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding transaction fees.", ex);
                return false;
            }
            return true;
        }

        public async Task<bool> AddUserPermissions(UserPermission userPermission)
        {
            try
            {
                string commandText = "INSERT INTO tblUserPermission (ProfileId, PageId, PageCatId) VALUES (@ProfileId, @PageId, @PageCatId)";

                // Parameters
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.VarChar, 50) { Value = userPermission.ProfileId },
                    new SqlParameter("@PageId", SqlDbType.Int) { Value = userPermission.PageId },
                    //new SqlParameter("@PageCatId", SqlDbType.Int){Value = userPermission.PageCatId ?? DBNull.Value}
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
