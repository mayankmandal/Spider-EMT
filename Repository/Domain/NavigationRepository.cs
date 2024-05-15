using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Spider_EMT.DAL;
using Spider_EMT.Models;
using Spider_EMT.Models.ViewModels;
using Spider_EMT.Repository.Skeleton;
using Spider_EMT.Utility;
using System.Collections.Generic;
using System.Data;
using static Spider_EMT.Utility.Constants;

namespace Spider_EMT.Repository.Domain
{
    public class NavigationRepository : INavigationRepository
    {
        public NavigationRepository()
        {

        }
        public CurrentUser GetCurrentUser()
        {
            try
            {
                string commandText = "SELECT UserId,Username,Userimgpath FROM tblCurrentUser";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                CurrentUser currentUser = null;
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
        public List<ProfileUser> GetAllProfileUsers()
        {
            try
            {
                string commandText = "SELECT UserId,FirstName,LastName,BirthDate FROM tblUsers";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<ProfileUser> profileUsers = new List<ProfileUser>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        ProfileUser profileUser = new ProfileUser
                        {
                            UserId = (int)row["UserId"],
                        };
                        profileUsers.Add(profileUser);
                    }
                }
                return profileUsers;
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
                throw new Exception("Error in Getting All Profiles.", ex);
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
                throw new Exception("Error in Getting All the Pages.", ex);
            }
        }
        public List<PageCategory> GetAllCategories()
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
        public ProfileSite GetCurrentUserProfile()
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
        public List<PageSite> GetCurrentUserPages()
        {
            try
            {
                string commandText = "SELECT DISTINCT tbp.* from tblPage tbp INNER JOIN tblUserPermission tup on tbp.PageId = tup.PageId INNER JOIN tblProfile tp on tup.ProfileId = tp.ProfileId INNER JOIN tblUserProfile tbup on tbup.ProfileId = tp.ProfileId INNER JOIN tblCurrentUser tcu on tbup.UserId = tcu.UserId";

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

        public List<PageSite> GetProfilePageData(string profileId)
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
        public List<PageCategory> GetCurrentUserCategories()
        {
            try
            {
                string commandText = "SELECT DISTINCT tbpc.*,tbp.PageId from tblPageCatagory tbpc INNER JOIN tblPage tbp on tbp.PageCatId = tbpc.PageCatId INNER JOIN tblUserPermission tup on tbp.PageId = tup.PageId INNER JOIN tblProfile tp on tup.ProfileId = tp.ProfileId INNER JOIN tblUserProfile tbup on tbup.ProfileId = tp.ProfileId INNER JOIN tblCurrentUser tcu on tbup.UserId = tcu.UserId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageCategory> pageCategories = new List<PageCategory>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageCategory pageCategory = new PageCategory
                        {
                            PageId = (int)row["PageId"],
                            CategoryName = row["CatagoryName"].ToString(),
                            PageCatId = (int)row["PageCatId"],
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
                throw new Exception("Error in Getting Current User Categories.", ex);
            }
        }
        public List<PageSite> GetNewUserPages()
        {
            try
            {
                string commandText = "SELECT DISTINCT tbp.PageUrl,tbp.PageId,tbp.PageDescription,tbp.PageCatId,tbp.MenuImgPath FROM tblPage tbp LEFT JOIN tblUserPermission tup ON tbp.PageId = tup.PageId LEFT JOIN tblProfile tp ON tup.ProfileId = tp.ProfileId LEFT JOIN tblUserProfile tbup ON tbup.ProfileId = tp.ProfileId LEFT JOIN tblCurrentUser tcu ON tbup.UserId = tcu.UserId WHERE tbp.PageId NOT IN ( SELECT DISTINCT tbp.PageId  FROM tblPage tbp  INNER JOIN tblUserPermission tup ON tbp.PageId = tup.PageId  INNER JOIN tblProfile tp ON tup.ProfileId = tp.ProfileId  INNER JOIN tblUserProfile tbup ON tbup.ProfileId = tp.ProfileId  INNER JOIN tblCurrentUser tcu ON tbup.UserId = tcu.UserId)";

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
                throw new Exception("Error in Getting new user pages.", ex);
            }
        }
        public List<PageCategory> GetNewUserCategories()
        {
            try
            {
                string commandText = "SELECT DISTINCT tbpc.*, tbp.PageId FROM tblPageCatagory tbpc INNER JOIN tblPage tbp ON tbp.PageCatId = tbpc.PageCatId INNER JOIN tblUserPermission tup ON tbp.PageId = tup.PageId INNER JOIN tblProfile tp ON tup.ProfileId = tp.ProfileId INNER JOIN tblUserProfile tbup ON tbup.ProfileId = tp.ProfileId INNER JOIN tblCurrentUser tcu ON tbup.UserId = tcu.UserId WHERE tbpc.PageCatId NOT IN (    SELECT DISTINCT tbpc.PageCatId FROM tblPageCatagory tbpc INNER JOIN tblPage tbp ON tbp.PageCatId = tbpc.PageCatId INNER JOIN tblUserPermission tup ON tbp.PageId = tup.PageId INNER JOIN tblProfile tp ON tup.ProfileId = tp.ProfileId INNER JOIN tblUserProfile tbup ON tbup.ProfileId = tp.ProfileId INNER JOIN tblCurrentUser tcu ON tbup.UserId = tcu.UserId)";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageCategory> pageCategories = new List<PageCategory>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageCategory pageCategory = new PageCategory
                        {
                            PageId = (int)row["PageId"],
                            CategoryName = row["CatagoryName"].ToString(),
                            PageCatId = (int)row["PageCatId"],
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
                throw new Exception("Error in Getting new user categories.", ex);
            }
        }
        public List<PageCategory> GetPageToCategories(List<int> pageList)
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
                throw new Exception("Error in Getting Current Profiles.", ex);
            }
        }
        public async Task<bool> CreateUserProfile(ProfileUser userProfileData)
        {
            try
            {
                int UserIdentity = 0;

                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewUserId", SqlDbType.Int) { Value = userProfileData.UserId },
                    new SqlParameter("@NewIdNumber", SqlDbType.VarChar, 20) { Value = userProfileData.IdNumber },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 100) { Value = userProfileData.FullName },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = userProfileData.Email },
                    new SqlParameter("@NewMobileNumber", SqlDbType.VarChar, 15) { Value = userProfileData.MobileNo },
                    new SqlParameter("@NewProfileId", SqlDbType.Int) { Value = userProfileData.ProfileSiteData.ProfileId },
                    new SqlParameter("@NewUserStatus", SqlDbType.VarChar, 20) { Value = userProfileData.UserStatus },
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(Constants.SP_AddUserProfile, CommandType.StoredProcedure, sqlParameters);
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
        public async Task<bool> CreateUserAccess(ProfilePagesAccessDTO profilePagesAccessDTO)
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

                bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
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
        public async Task<bool> UpdateUserAccess(ProfilePagesAccessDTO profilePagesAccessDTO)
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

                bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
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
        public List<PageSite> GetCategoryToPages(int categoryId)
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
        public async Task<bool> CreateNewCategory(CategoryPagesAccessDTO categoryPagesAccessDTO)
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
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(Constants.SP_AddNewCategory, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeletePageCategoryMap, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddPageCategoryMap, CommandType.StoredProcedure, sqlParameters);
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

        public async Task<bool> UpdateCategory(CategoryPagesAccessDTO categoryPagesAccessDTO)
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
                isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeletePageCategoryMap, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddPageCategoryMap, CommandType.StoredProcedure, sqlParameters);
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
        public async Task<bool> AssignProfileCategories(ProfileCategoryAccessDTO profileCategoryAccessDTO)
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

                bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
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
                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
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
    }
}
