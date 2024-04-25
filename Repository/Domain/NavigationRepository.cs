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
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            BirthDate = (DateTime)row["BirthDate"],
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
                            ProfileId = (int)row["ProfileId"],
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
                            PageCatId = (int)row["PageCatId"],
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
                            PageCatId = (int)row["PageCatId"],
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

                    bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
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

                    bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while adding User Profile - SQL Exception.", sqlEx);
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding User Profile.", ex);
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateUserProfile(ProfileSite profile, List<PageSite> pages, List<PageCategory> pageCategories)
        {
            try
            {
                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.Int) { Value = profile.ProfileId },
                    new SqlParameter("@NewProfileName", SqlDbType.VarChar, 50) { Value = profile.ProfileName }
                };

                // Execute the command
                bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_UpdateUserProfile, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }

                sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@ProfileId", SqlDbType.Int) { Value = profile.ProfileId }
                };
                isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeleteUserPermission, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }

                // User Permission Allotment based on categories list provided already having associated pages linked
                foreach (PageCategory pageCategory in pageCategories)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profile.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageCategory.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = pageCategory.PageCatId}
                    };

                    isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddUserPermission, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
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
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profile.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pagesite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = pagesite.PageCatId}
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
                throw new Exception("Error while updating User Profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating User Profile.", ex);
            }
            return true;
        }
        public async Task<bool> AddNewUserProfile(List<ProfileSite> profiles, List<ProfileUser> profileUsers)
        {
            try
            {
                SqlParameter[] sqlParameters;
                // Deletion of existing access for profiles for a user
                foreach (ProfileUser profileUser in profileUsers)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@UserId", SqlDbType.Int){Value = profileUser.UserId},
                    };

                    bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_DeleteUserProfile, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }
                // User Profile Access Allotment
                foreach (ProfileUser profileUser in profileUsers)
                {
                    foreach (ProfileSite profileSite in profiles)
                    {
                        sqlParameters = new SqlParameter[]
                        {
                            new SqlParameter("@ProfileId", SqlDbType.Int){Value = profileSite.ProfileId},
                            new SqlParameter("@UserId", SqlDbType.Int){Value = profileUser.UserId},
                        };

                        bool isFailure = SqlDBHelper.ExecuteNonQuery(Constants.SP_AddNewUserProfile, CommandType.StoredProcedure, sqlParameters);
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
    }
}
