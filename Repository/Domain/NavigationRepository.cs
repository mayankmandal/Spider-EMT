using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using System.Data;
using static Spider_EMT.Utility.Constants;

namespace Spider_EMT.Repository.Domain
{
    public class NavigationRepository : INavigationRepository
    {
        public NavigationRepository()
        {

        }
        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            try
            {
                string commandText = "SELECT UserId,Username,Userimgpath FROM tblCurrentUser";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                CurrentUser currentUser = new CurrentUser();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        currentUser = new CurrentUser
                        {
                            UserId = (int)row["UserId"],
                            UserName = row["Username"].ToString(),
                            UserImgPath = row["Userimgpath"].ToString()
                        };
                    }
                }
                return currentUser;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting Current User Profile.", ex);
            }
        }
        
        public async Task<List<ProfileSite>> GetAllProfilesAsync()
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
                throw new Exception("Error in Getting All Profiles.", ex);
            }
        }
        public async Task<List<PageSite>> GetAllPagesAsync()
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
                throw new Exception("Error in Getting All the Pages.", ex);
            }
        }
        public async Task<List<PageCategory>> GetAllCategoriesAsync()
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
                throw new Exception("Error in Get All the Page Categories.", ex);
            }
        }
        public async Task<ProfileUser> GetCurrentUserDetailsAsync()
        {
            try
            {
                string commandText = "select u.*, tp.ProfileId,tp.ProfileName from tblUsers u INNER JOIN tblCurrentUser cu on u.UserId = cu.UserId INNER JOIN tblUserProfile tup on tup.UserId = u.UserId INNER JOIN tblProfile tp on tp.ProfileId = tup.ProfileId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                ProfileUser profileUser = new ProfileUser();
                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    profileUser = new ProfileUser
                    {
                        UserId = (int)dataRow["UserId"],
                        IdNumber = dataRow["IdNumber"].ToString(),
                        FullName = dataRow["FullName"].ToString(),
                        Email = dataRow["Email"].ToString(),
                        MobileNo = dataRow["MobileNo"].ToString(),
                        ProfileSiteData = new ProfileSite
                        {
                            ProfileId = (int)dataRow["ProfileId"],
                            ProfileName = dataRow["ProfileName"].ToString()
                        },
                        UserStatus = dataRow["Status"].ToString(),
                    };
                }
                return profileUser;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting All Users Data.", ex);
            }
        }
        public async Task<ProfileSite> GetCurrentUserProfileAsync()
        {
            try
            {
                string commandText = "SELECT tp.* FROM tblProfile tp INNER JOIN tblUserProfile tbup on tp.ProfileId = tbup.ProfileId INNER JOIN tblCurrentUser tcu on tbup.UserId = tcu.UserId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                ProfileSite profileSite = new ProfileSite();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        profileSite = new ProfileSite
                        {
                            ProfileName = row["ProfileName"].ToString(),
                        };
                    }
                }
                return profileSite;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting Current User Profile.", ex);
            }
        }
        public async Task<List<PageSite>> GetCurrentUserPagesAsync()
        {
            try
            {
                string commandText = "SELECT * FROM vwUserPageAccess";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageSite> pages = new List<PageSite>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageSite page = new PageSite
                        {
                            PageId = (int)row["PageId"],
                            MenuImgPath = row["MenuImgPath"].ToString(),
                            PageDescription = row["PageDescription"].ToString(),
                            PageUrl = row["PageUrl"].ToString(),
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
                throw new Exception("Error in Getting Current User Pages.", ex);
            }
        }

        public async Task<List<PageSite>> GetProfilePageDataAsync(string profileId)
        {
            try
            {
                string commandText = $"SELECT DISTINCT tbp.* from tblPage tbp INNER JOIN tblUserPermission tup on tbp.PageId = tup.PageId WHERE tup.ProfileId = {profileId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageSite> pages = new List<PageSite>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageSite page = new PageSite
                        {
                            PageId = (int)row["PageId"],
                            MenuImgPath = row["MenuImgPath"].ToString(),
                            PageDescription = row["PageDescription"].ToString(),
                            PageUrl = row["PageUrl"].ToString(),
                        };
                        pages.Add(page);
                    }
                }
                return pages;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.\n", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting associated pages for profile.\n", ex);
            }
        }
        public async Task<List<CategoriesSetDTO>> GetCurrentUserCategoriesAsync()
        {
            try
            {
                string commandText = "SELECT tbpc.PageCatId,tbpc.CatagoryName,tbp.PageId, tbp.PageDescription,tbp.PageUrl,tbp.MenuImgPath from tblPageCatagory tbpc  INNER JOIN tblPageCategoryMap tcm on tcm.PageCatId = tbpc.PageCatId INNER JOIN tblPage tbp on tbp.PageId = tcm.PageId  INNER JOIN tblUserPermission tup on tup.PageCatId = tbpc.PageCatId  INNER JOIN tblUserProfile tbup on tbup.ProfileId = tup.ProfileId  INNER JOIN tblCurrentUser tcu on tcu.UserId = tbup.UserId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<CategoriesSetDTO> categoriesSet = new List<CategoriesSetDTO>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CategoriesSetDTO category = new CategoriesSetDTO
                        {
                            PageCatId = (int)row["PageCatId"],
                            CatagoryName = row["CatagoryName"].ToString(),
                            PageId = (int)row["PageId"],
                            PageDescription = row["PageDescription"].ToString(),
                            PageUrl = row["PageUrl"].ToString(),
                            MenuImgPath = row["MenuImgPath"].ToString()
                        };
                        categoriesSet.Add(category);
                    }
                }
                return categoriesSet;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting Current User Categories.", ex);
            }
        }
        
        public async Task<List<PageCategory>> GetPageToCategoriesAsync(List<int> pageList)
        {
            try
            {
                List<PageCategory> pageCategories = new List<PageCategory>();

                foreach (int pageId in pageList)
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
                throw new Exception("Error in Getting Page to Categories.", ex);
            }
        }
        public async Task<List<CurrentUserProfileViewModel>> GetCurrentProfilesAsync()
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
                        userProfiles.Add(new CurrentUserProfileViewModel
                        {
                            ProfileId = (int)row["ProfileId"],
                            ProfileName = row["ProfileName"].ToString(),
                            UserId = (int)row["UserId"],
                        });
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
                throw new Exception("Error in Getting Current Profiles.", ex);
            }
        }
        public async Task<bool> CreateUserProfileAsync(ProfileUser userProfileData)
        {
            try
            {
                int UserId = 0;
                bool isFailure = false;
                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewIdNumber", SqlDbType.VarChar, 20) { Value = userProfileData.IdNumber },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 100) { Value = userProfileData.FullName },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = userProfileData.Email },
                    new SqlParameter("@NewMobileNumber", SqlDbType.VarChar, 15) { Value = userProfileData.MobileNo },
                    new SqlParameter("@NewProfileId", SqlDbType.Int) { Value = userProfileData.ProfileSiteData.ProfileId },
                    new SqlParameter("@NewUserStatus", SqlDbType.VarChar, 20) { Value = userProfileData.UserStatus },
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_AddNewUser, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        UserId = (int)dataRow["UserId"];
                    }
                    else
                    {
                        return false;
                    }
                }
                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.Int){Value = userProfileData.ProfileSiteData.ProfileId},
                    new SqlParameter("@UserId", SqlDbType.Int){Value = UserId},
                };
                isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddNewUserProfile, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while adding User Profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding User Profile.", ex);
            }
            return true;
        }
        public async Task<bool> UpdateUserProfileAsync(ProfileUser userProfileData)
        {
            try
            {
                int NumberOfRowsAffected = 0;
                // User Profile Updation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userProfileData.UserId },
                    new SqlParameter("@NewIdNumber", SqlDbType.VarChar, 20) { Value = userProfileData.IdNumber },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 100) { Value = userProfileData.FullName },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = userProfileData.Email },
                    new SqlParameter("@NewMobileNumber", SqlDbType.VarChar, 15) { Value = userProfileData.MobileNo },
                    new SqlParameter("@NewProfileId", SqlDbType.Int) { Value = userProfileData.ProfileSiteData.ProfileId },
                    new SqlParameter("@NewUserStatus", SqlDbType.VarChar, 20) { Value = userProfileData.UserStatus },
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_UpdateUser, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        NumberOfRowsAffected = (int)dataRow["RowsAffected"];
                        if (NumberOfRowsAffected < 0) 
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while adding User Profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding User Profile.", ex);
            }
            return true;
        }
        public async Task<bool> CreateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                SqlParameter[] sqlParameters;
                bool isFailure;
                int UserIdentity = 0;
                
                if (profilePagesAccessDTO.ProfileData.ProfileId > 0)
                {
                    // Deletion of existing access for profiles for a user
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@ProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                        new SqlParameter("@State", SqlDbType.Int){Value = UserPermissionStates.PageIdOnly},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                    // User Profile Access Allotment
                    foreach (PageSite pageSite in profilePagesAccessDTO.PagesList)
                    {
                        sqlParameters = new SqlParameter[]
                        {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                        };
                        isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                        if (isFailure)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileName", SqlDbType.VarChar,50){Value = profilePagesAccessDTO.ProfileData.ProfileName},
                    };
                    // Execute the command
                    List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_AddNewProfile, CommandType.StoredProcedure, sqlParameters);
                    if (tables.Count > 0)
                    {
                        DataTable dataTable = tables[0];
                        if (dataTable.Rows.Count > 0)
                        {
                            DataRow dataRow = dataTable.Rows[0];
                            UserIdentity = (int)dataRow["UserIdentity"];
                        }
                        else
                        {
                            return false;
                        }
                    }

                    // User Profile Access Allotment
                    foreach (PageSite pageSite in profilePagesAccessDTO.PagesList)
                    {
                        sqlParameters = new SqlParameter[]
                        {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = UserIdentity},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                        };
                        isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                        if (isFailure)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding relationship between new user profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding relationship between new user profile.", ex);
            }
            return true;
        }
        public async Task<bool> UpdateUserAccessAsync(ProfilePagesAccessDTO profilePagesAccessDTO)
        {
            try
            {
                SqlParameter[] sqlParameters;
                // Deletion of existing access for profiles for a user
                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                    new SqlParameter("@State", SqlDbType.Int){Value = UserPermissionStates.PageIdOnly},
                };

                bool isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }
                // User Profile Access Allotment
                foreach (PageSite pageSite in profilePagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding relationship between new user profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding relationship between new user profile.", ex);
            }
            return true;
        }
        public async Task<List<PageSite>> GetCategoryToPagesAsync(int categoryId)
        {
            try
            {
                List<PageSite> pages = new List<PageSite>();

                string commandText = "SELECT tbp.* from tblPage tbp INNER JOIN tblPageCategoryMap tbm on tbp.PageId = tbm.PageId INNER JOIN tblPageCatagory tbc on tbm.PageCatId = tbc.PageCatId WHERE tbc.PageCatId = @CategoryId";
                SqlParameter[] sqlParameter =
                {
                        new SqlParameter("@CategoryId", SqlDbType.Int){Value = categoryId}
                    };
                DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(commandText, CommandType.Text, sqlParameter);
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageSite pageSite = new PageSite
                        {
                            PageId = (int)row["PageId"],
                            PageUrl = row["PageUrl"].ToString(),
                            PageDescription = row["PageDescription"].ToString(),
                            MenuImgPath = row["MenuImgPath"].ToString(),

                        };
                        pages.Add(pageSite);
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
                throw new Exception("Error in Getting Page to Categories.", ex);
            }
        }
        public async Task<bool> CreateNewCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                int UserIdentity = 0;
                bool isFailure = false;

                SqlParameter[] sqlParameters;
                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewCategoryName", SqlDbType.VarChar,50){Value = categoryPagesAccessDTO.PageCategoryData.CategoryName}
                };

                // Creation of New Category
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_AddNewCategory, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        UserIdentity = (int)dataRow["UserIdentity"];
                    }
                    else
                    {
                        return false;
                    }
                }

                // Deletion
                foreach (PageSite pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@State", SqlDbType.VarChar, 2){Value = PageCategoryMapStates.PageIdOnly},
                        new SqlParameter("@PageId", SqlDbType.Int){Value = pageSite.PageId},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeletePageCategoryMap, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }

                // Insertion
                foreach (PageSite pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = UserIdentity},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddPageCategoryMap, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding relationship between new user profile and pages - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding relationship between new user profile and pages.", ex);
            }
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryPagesAccessDTO categoryPagesAccessDTO)
        {
            try
            {
                int UserIdentity = categoryPagesAccessDTO.PageCategoryData.PageCatId;
                bool isFailure = false;

                SqlParameter[] sqlParameters;

                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@State", SqlDbType.VarChar, 2){Value = PageCategoryMapStates.PageCategoryIdOnly},
                    new SqlParameter("@PageCatId", SqlDbType.Int){Value = UserIdentity},
                };
                isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeletePageCategoryMap, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }

                // Insertion
                foreach (PageSite pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = UserIdentity},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddPageCategoryMap, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error Updating relationship between existing profile and pages - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Updating relationship between existing profile and pages.", ex);
            }
            return true;
        }
        public async Task<bool> AssignProfileCategoriesAsync(ProfileCategoryAccessDTO profileCategoryAccessDTO)
        {
            try
            {
                SqlParameter[] sqlParameters;
                // Deletion of existing access for profiles for a user
                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.Int){Value = profileCategoryAccessDTO.ProfileSiteData.ProfileId},
                    new SqlParameter("@State", SqlDbType.Int){Value = UserPermissionStates.PageCategoryIdOnly},
                };

                bool isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }
                // User Profile Access Allotment
                foreach (PageCategory pageCategory in profileCategoryAccessDTO.PageCategories)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profileCategoryAccessDTO.ProfileSiteData.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = DBNull.Value},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = pageCategory.PageCatId},
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding relationship between new user profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding relationship between new user profile.", ex);
            }
            return true;
        }

        public async Task<List<ProfileUser>> SearchUserDetailsByCriteriaAsync(string criteriaText, string InputText)
        {
            try
            {
                SqlParameter[] sqlParameters;
                int InputValue = 0;
                foreach (var d in Enum.GetValues(typeof(SearchByTextStates)))
                {
                    if(criteriaText == d.ToString())
                    {
                        InputValue = (int)d;
                        break;
                    }
                }
                
                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TextCriteria", SqlDbType.Int){Value = InputValue},
                    new SqlParameter("@InputText", SqlDbType.VarChar, 100){Value = InputText},
                };

                DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(SP_SearchUserByTextCriteria, CommandType.StoredProcedure, sqlParameters);
                List<ProfileUser> profileUserLst = new List<ProfileUser>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach(DataRow dataRow in dataTable.Rows)
                    {
                        ProfileUser profileUser = new ProfileUser
                        {
                            UserId = (int)dataRow["UserId"],
                            IdNumber = dataRow["IdNumber"].ToString(),
                            FullName = dataRow["FullName"].ToString(),
                            Email = dataRow["Email"].ToString(),
                            MobileNo = dataRow["MobileNo"].ToString(),
                            ProfileSiteData = new ProfileSite
                            {
                                ProfileId = (int)dataRow["ProfileId"],
                                ProfileName = dataRow["ProfileName"].ToString()
                            },
                            UserStatus = dataRow["Status"].ToString(),
                        };
                        profileUserLst.Add(profileUser);
                    }
                }
                return profileUserLst;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error adding relationship between new user profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding relationship between new user profile.", ex);
            }
        }
    }
}
