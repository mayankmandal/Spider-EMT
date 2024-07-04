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
        private readonly IConfiguration _configuration;
        private readonly CurrentUser _currentUser;
        public NavigationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _currentUser = new CurrentUser
            {
                UserId = Int32.Parse(_configuration["RecentUserId"])
            };

        }
        private object GetDbValue<T>(T newValue, T existingValue)
        {
            if (newValue == null || newValue.Equals(DBNull.Value))
            {
                return DBNull.Value;
            }
            // Handle string case
            if (typeof(T) == typeof(string))
            {
                if (string.IsNullOrEmpty(newValue as string) || (newValue as string) == (existingValue as string))
                {
                    return DBNull.Value;
                }
            }
            // Handle other types (int?, bool?, etc.)
            else if (newValue.Equals(existingValue))
            {
                return DBNull.Value;
            }
            return newValue;
        }
        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            try
            {
                string commandText = $"SELECT UserId,Username,Userimgpath FROM tblUsers where UserId = {_currentUser.UserId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                CurrentUser currentUser = new CurrentUser();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        currentUser = new CurrentUser
                        {
                            UserId = _currentUser.UserId,
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
        public async Task<List<ProfileUserAPIVM>> GetAllUsersDataAsync()
        {
            try
            {
                string commandText = "select tp.ProfileId,tp.ProfileName, u.UserId, u.IdNumber, u.FullName, u.Email, u.MobileNo, u.Username, u.Userimgpath, u.IsActive, u.IsActiveDirectoryUser, u.ChangePassword from tblUsers u INNER JOIN tblUserProfile tup on tup.UserId = u.UserId INNER JOIN tblProfile tp on tp.ProfileId = tup.ProfileId";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<ProfileUserAPIVM> users = new List<ProfileUserAPIVM>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        ProfileUserAPIVM profileUser = new ProfileUserAPIVM
                        {
                            UserId = Convert.ToInt32(dataRow["UserId"]),
                            IdNumber = Convert.ToInt64(dataRow["IdNumber"]),
                            FullName = dataRow["FullName"].ToString(),
                            Email = dataRow["Email"].ToString(),
                            MobileNo = Convert.ToInt64(dataRow["MobileNo"]),
                            ProfileSiteData = new ProfileSiteVM
                            {
                                ProfileId = Convert.ToInt32(dataRow["ProfileId"]),
                                ProfileName = dataRow["ProfileName"].ToString()
                            },
                            Username = dataRow["Username"].ToString(),
                            Userimgpath = dataRow["Userimgpath"].ToString(),
                            IsActive = Convert.ToBoolean(dataRow["IsActive"]),
                            IsActiveDirectoryUser = Convert.ToBoolean(dataRow["IsActiveDirectoryUser"]),
                            ChangePassword = Convert.ToBoolean(dataRow["ChangePassword"]),
                        };
                        users.Add(profileUser);
                    }
                }
                return users;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting All Users Details.", ex);
            }
        }

        public async Task<List<ProfileSite>> GetAllProfilesAsync()
        {
            try
            {
                string commandText = "SELECT ProfileId, ProfileName, CreateDate, CreateUserId, UpdateDate, UpdateUserId FROM tblProfile";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<ProfileSite> profiles = new List<ProfileSite>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        ProfileSite profile = new ProfileSite
                        {
                            ProfileId = (int)row["ProfileId"],
                            ProfileName = row["ProfileName"].ToString(),
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
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
                string commandText = "SELECT PageId, PageUrl, PageDescription, CreateDate, CreateUserId, UpdateDate, UpdateUserId FROM tblPage";

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
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
                            isSelected = false
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
                string commandText = "SELECT PageCatId,CatagoryName, CreateDate, CreateUserId, UpdateDate, UpdateUserId FROM tblPageCatagory";

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
                            PageId = 0,
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
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
        public async Task<ProfileUserAPIVM> GetCurrentUserDetailsAsync()
        {
            try
            {
                string commandText = $"select u.*, tp.ProfileId,tp.ProfileName from tblUsers u INNER JOIN tblUserProfile tup on tup.UserId = u.UserId INNER JOIN tblProfile tp on tp.ProfileId = tup.ProfileId WHERE u.UserId = {_currentUser.UserId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                ProfileUserAPIVM profileUser = new ProfileUserAPIVM();
                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    profileUser = new ProfileUserAPIVM
                    {
                        UserId = Convert.ToInt32(dataRow["UserId"]),
                        IdNumber = Convert.ToInt64(dataRow["IdNumber"]),
                        FullName = dataRow["FullName"].ToString(),
                        Email = dataRow["Email"].ToString(),
                        MobileNo = Convert.ToInt64(dataRow["MobileNo"]),
                        ProfileSiteData = new ProfileSiteVM
                        {
                            ProfileId = Convert.ToInt32(dataRow["ProfileId"]),
                            ProfileName = dataRow["ProfileName"].ToString()
                        },
                        Username = dataRow["Username"].ToString(),
                        Userimgpath = dataRow["Userimgpath"].ToString(),
                        IsActive = Convert.ToBoolean(dataRow["IsActive"]),
                        IsActiveDirectoryUser = Convert.ToBoolean(dataRow["IsActiveDirectoryUser"]),
                        ChangePassword = Convert.ToBoolean(dataRow["ChangePassword"]),
                        CreateUserId = (int)dataRow["CreateUserId"],
                        UpdateUserId = (int)dataRow["UpdateUserId"]
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
                string commandText = $"SELECT tp.* FROM tblProfile tp INNER JOIN tblUserProfile tbup on tp.ProfileId = tbup.ProfileId WHERE tbup.UserId = {_currentUser.UserId}";

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
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
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
        public async Task<List<PageSiteVM>> GetCurrentUserPagesAsync()
        {
            try
            {
                string commandText = $"SELECT * FROM vwUserPageAccess where UserId = {_currentUser.UserId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<PageSiteVM> pages = new List<PageSiteVM>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        PageSiteVM page = new PageSiteVM
                        {
                            PageId = (int)row["PageId"],
                            PageDescription = row["PageDescription"].ToString(),
                            PageUrl = row["PageUrl"].ToString(),
                            isSelected = true
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
                            PageDescription = row["PageDescription"].ToString(),
                            PageUrl = row["PageUrl"].ToString(),
                            isSelected = false,
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
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
                string commandText = $"SELECT * FROM vwUserPagesData where UserId = {_currentUser.UserId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                List<CategoriesSetDTO> categoriesSet = new List<CategoriesSetDTO>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        CategoriesSetDTO category = new CategoriesSetDTO
                        {
                            PageCatId = row["PageCatId"] != DBNull.Value ? (int)row["PageCatId"] : 0,
                            CatagoryName = row["CatagoryName"] != DBNull.Value ? row["CatagoryName"].ToString() : string.Empty,
                            PageId = row["PageId"] != DBNull.Value ? (int)row["PageId"] : 0,
                            PageDescription = row["PageDescription"] != DBNull.Value ? row["PageDescription"].ToString() : string.Empty,
                            PageUrl = row["PageUrl"] != DBNull.Value ? row["PageUrl"].ToString() : string.Empty,
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
                    string commandText = "SELECT DISTINCT tpc.PageCatId, tpc.CatagoryName, tpcm.CreateDate, tpcm.CreateUserId, tpcm.UpdateDate, tpcm.UpdateUserId from tblPageCatagory tpc INNER JOIN tblPageCategoryMap tpcm on tpc.PageCatId = tpcm.PageCatId INNER JOIN tblPage tp on tpcm.PageId = tp.PageId WHERE tp.PageId = = @PageId";
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
                                PageId = pageId,
                                CreateDate = (DateTime)row["CreateDate"],
                                CreateUserId = (int)row["CreateUserId"],
                                UpdateDate = (DateTime)row["UpdateDate"],
                                UpdateUserId = (int)row["UpdateUserId"],
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
                string commandText = $"SELECT pro.ProfileId,ProfileName,usrpro.UserId from tblProfile pro INNER JOIN tblUserProfile usrpro on pro.ProfileId = usrpro.ProfileId WHERE usrpro.UserId = {_currentUser.UserId}";

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
                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@NewIdNumber", SqlDbType.VarChar, 10) { Value = userProfileData.IdNumber },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 200) { Value = userProfileData.FullName },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = userProfileData.Email },
                    new SqlParameter("@NewMobileNumber", SqlDbType.VarChar, 15) { Value = userProfileData.MobileNo },
                    new SqlParameter("@NewProfileId", SqlDbType.Int) { Value = userProfileData.ProfileSiteData.ProfileId },
                    new SqlParameter("@NewUsername", SqlDbType.VarChar, 100) { Value = userProfileData.Username },
                    new SqlParameter("@NewUserimgpath", SqlDbType.VarChar, 255) { Value = userProfileData.Userimgpath },
                    new SqlParameter("@NewPasswordHash", SqlDbType.VarChar, 255) { Value = userProfileData.PasswordHash },
                    new SqlParameter("@NewPasswordSalt", SqlDbType.VarChar, 255) { Value = userProfileData.PasswordSalt },
                    new SqlParameter("@NewIsActive", SqlDbType.Bit) { Value = userProfileData.IsActive ? 1 : 0 },
                    new SqlParameter("@NewIsActiveDirectoryUser", SqlDbType.Bit) { Value = userProfileData.IsActiveDirectoryUser ? 1 : 0  },
                    new SqlParameter("@NewChangePassword", SqlDbType.Bit) { Value = userProfileData.ChangePassword ? 1 : 0 },
                    new SqlParameter("@NewLastLoginActivity", SqlDbType.DateTime) { Value = userProfileData.LastLoginActivity.HasValue ? (object)userProfileData.LastLoginActivity.Value : DBNull.Value }, // Handle nullable DateTime
                    new SqlParameter("@NewCreateUserId", SqlDbType.Int) { Value = _currentUser.UserId },
                    new SqlParameter("@NewUpdateUserId", SqlDbType.Int) { Value = _currentUser.UserId }
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
                    if (UserId <= 0)
                    {
                        return false;
                    }
                }
                else
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
        public async Task<string> UpdateUserProfileAsync(ProfileUserAPIVM userProfileData)
        {
            try
            {
                int NumberOfRowsAffected = 0;

                ProfileUserAPIVM profileUserExisting = new ProfileUserAPIVM();
                string commandText = $"SELECT u.IdNumber, u.FullName, u.Email, u.MobileNo, u.Username, u.Userimgpath, u.IsActive, u.IsActiveDirectoryUser, u.ChangePassword, u.ProfileId from tblUsers u WHERE u.UserId = {_currentUser.UserId}";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    profileUserExisting = new ProfileUserAPIVM
                    {
                        IdNumber = Convert.ToInt64(dataRow["IdNumber"]),
                        FullName = dataRow["FullName"].ToString(),
                        Email = dataRow["Email"].ToString(),
                        MobileNo = Convert.ToInt64(dataRow["MobileNo"]),
                        ProfileSiteData = new ProfileSiteVM
                        {
                            ProfileId = Convert.ToInt32(dataRow["ProfileId"]),
                        },
                        Username = dataRow["Username"].ToString(),
                        Userimgpath = dataRow["Userimgpath"].ToString(),
                        IsActive = Convert.ToBoolean(dataRow["IsActive"]),
                        IsActiveDirectoryUser = Convert.ToBoolean(dataRow["IsActiveDirectoryUser"]),
                        ChangePassword = Convert.ToBoolean(dataRow["ChangePassword"]),

                    };
                }

                // User Profile Updation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userProfileData.UserId },
                    new SqlParameter("@NewIdNumber", SqlDbType.VarChar, 10) { Value = GetDbValue(userProfileData.IdNumber, profileUserExisting.IdNumber) },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 200) { Value = GetDbValue(userProfileData.FullName, profileUserExisting.FullName) },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = GetDbValue(userProfileData.Email, profileUserExisting.Email) },
                    new SqlParameter("@NewMobileNumber", SqlDbType.VarChar, 15) { Value = GetDbValue(userProfileData.MobileNo, profileUserExisting.MobileNo) },
                    new SqlParameter("@NewProfileId", SqlDbType.Int) { Value = GetDbValue(userProfileData.ProfileSiteData.ProfileId, profileUserExisting.ProfileSiteData.ProfileId) },
                    new SqlParameter("@NewUsername", SqlDbType.VarChar, 100) { Value = GetDbValue(userProfileData.Username, profileUserExisting.Username) },
                    new SqlParameter("@NewUserimgpath", SqlDbType.VarChar, 255) { Value = GetDbValue(userProfileData.Userimgpath, profileUserExisting.Userimgpath) },
                    new SqlParameter("@NewIsActive", SqlDbType.Bit) { Value = GetDbValue(userProfileData.IsActive, profileUserExisting.IsActive) },
                    new SqlParameter("@NewIsActiveDirectoryUser", SqlDbType.Bit) { Value = GetDbValue(userProfileData.IsActiveDirectoryUser, profileUserExisting.IsActiveDirectoryUser) },
                    new SqlParameter("@NewChangePassword", SqlDbType.Bit) { Value = GetDbValue(userProfileData.ChangePassword, profileUserExisting.ChangePassword) },
                    new SqlParameter("@NewUpdateUserId", SqlDbType.Int) { Value = _currentUser.UserId }
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_UpdateUser, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        NumberOfRowsAffected = (int)dataRow["RowsAffected"];
                        if (NumberOfRowsAffected < 0)
                        {
                            return MagicString;
                        }
                    }
                    else
                    {
                        return MagicString;
                    }
                }
                return profileUserExisting.Userimgpath;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while adding User Profile - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding User Profile.", ex);
            }
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
                    foreach (PageSiteVM pageSite in profilePagesAccessDTO.PagesList)
                    {
                        sqlParameters = new SqlParameter[]
                        {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId},
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
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId},
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
                    foreach (PageSiteVM pageSite in profilePagesAccessDTO.PagesList)
                    {
                        sqlParameters = new SqlParameter[]
                        {
                            new SqlParameter("@NewProfileId", SqlDbType.Int){Value = UserIdentity},
                            new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                            new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                            new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                            new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId},
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
                foreach (PageSiteVM pageSite in profilePagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewProfileId", SqlDbType.Int){Value = profilePagesAccessDTO.ProfileData.ProfileId},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = DBNull.Value},
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId}
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
                            CreateDate = (DateTime)row["CreateDate"],
                            CreateUserId = (int)row["CreateUserId"],
                            UpdateDate = (DateTime)row["UpdateDate"],
                            UpdateUserId = (int)row["UpdateUserId"],
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
                    new SqlParameter("@NewCategoryName", SqlDbType.VarChar,50){Value = categoryPagesAccessDTO.PageCategoryData.CategoryName},
                    new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                    new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId},
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
                foreach (PageSiteVM pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@State", SqlDbType.VarChar, 2){Value = PageCategoryMapStates.PageIdOnly},
                        new SqlParameter("@PageId", SqlDbType.Int){Value = pageSite.PageId}
                    };
                    isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeletePageCategoryMap, CommandType.StoredProcedure, sqlParameters);
                    if (isFailure)
                    {
                        return false;
                    }
                }

                // Insertion
                foreach (PageSiteVM pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = UserIdentity},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId},
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
                foreach (PageSiteVM pageSite in categoryPagesAccessDTO.PagesList)
                {
                    sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@NewPageCatId", SqlDbType.Int){Value = UserIdentity},
                        new SqlParameter("@NewPageId", SqlDbType.Int){Value = pageSite.PageId},
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int){Value = _currentUser.UserId},
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int){Value = _currentUser.UserId}
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
                        new SqlParameter("@NewCreateUserId", SqlDbType.Int) { Value = _currentUser.UserId },
                        new SqlParameter("@NewUpdateUserId", SqlDbType.Int) { Value = _currentUser.UserId }
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

        public async Task<List<ProfileUserAPIVM>> SearchUserDetailsByCriteriaAsync(string criteriaText, string InputText)
        {
            try
            {
                SqlParameter[] sqlParameters;
                int InputValue = 0;
                foreach (var d in Enum.GetValues(typeof(SearchByTextStates)))
                {
                    if (criteriaText == d.ToString())
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
                List<ProfileUserAPIVM> profileUserLst = new List<ProfileUserAPIVM>();
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        ProfileUserAPIVM profileUserAPIVM = new ProfileUserAPIVM
                        {
                            UserId = Convert.ToInt32(dataRow["UserId"]),
                            IdNumber = Convert.ToInt64(dataRow["IdNumber"]),
                            FullName = dataRow["FullName"].ToString(),
                            Email = dataRow["Email"].ToString(),
                            MobileNo = Convert.ToInt64(dataRow["MobileNo"]),
                            ProfileSiteData = new ProfileSiteVM
                            {
                                ProfileId = Convert.ToInt32(dataRow["ProfileId"]),
                                ProfileName = dataRow["ProfileName"].ToString()
                            },
                            Username = dataRow["Username"].ToString(),
                            IsActive = Convert.ToBoolean(dataRow["IsActive"]),
                            IsActiveDirectoryUser = Convert.ToBoolean(dataRow["IsActiveDirectoryUser"]),
                            ChangePassword = Convert.ToBoolean(dataRow["ChangePassword"]),
                            Userimgpath = dataRow["Userimgpath"].ToString()
                        };
                        profileUserLst.Add(profileUserAPIVM);
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
        public async Task<bool> DeleteEntityAsync(int deleteId, string deleteType)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", SqlDbType.Int){Value = deleteId},
                    new SqlParameter("@Type", SqlDbType.VarChar, 10){Value = deleteType},
                };

                bool isFailure = SqlDBHelper.ExecuteNonQuery(SP_DeleteEntityRecord, CommandType.StoredProcedure, sqlParameters);
                if (isFailure)
                {
                    return false;
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error deleting record for {deleteType} - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting record for {deleteType}.", ex);
            }
        }
        public async Task<ProfileUserAPIVM> GetSettingsDataAsync()
        {
            try
            {
                ProfileUserAPIVM userSettings = new ProfileUserAPIVM();
                string commandText = $"SELECT tu.UserId, tu.Username, tu.Userimgpath, tu.FullName, tu.Email from tblUsers tu where tu.UserId = {_currentUser.UserId}";

                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    userSettings = new ProfileUserAPIVM
                    {
                        UserId = dataRow["UserId"] != DBNull.Value ? (int)dataRow["UserId"] : 0,
                        FullName = dataRow["FullName"] != DBNull.Value ? dataRow["FullName"].ToString() : string.Empty,
                        Email = dataRow["Email"] != DBNull.Value ? dataRow["Email"].ToString() : string.Empty,
                        Username = dataRow["Username"] != DBNull.Value ? dataRow["Username"].ToString() : string.Empty,
                        Userimgpath = dataRow["Userimgpath"] != DBNull.Value ? dataRow["Userimgpath"].ToString() : string.Empty,
                    };
                }
                return userSettings;
            }
            catch (SqlException sqlEx)
            {
                // Log or handle SQL exceptions
                throw new Exception("Error executing SQL command.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("Error in Getting Settings.", ex);
            }
        }
        public async Task<string> UpdateSettingsDataAsync(ProfileUser userSettings)
        {
            try
            {
                int NumberOfRowsAffected = 0;
                ProfileUser profileUserExisting = new ProfileUser();
                string commandText = $"SELECT u.Username, u.FullName, u.Email, u.Userimgpath, u.PasswordHash, u.PasswordSalt, u.UpdateUserId from tblUsers u WHERE u.UserId = {_currentUser.UserId}";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(commandText, CommandType.Text);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    profileUserExisting = new ProfileUser
                    {
                        Username = dataRow["Username"].ToString(),
                        FullName = dataRow["FullName"].ToString(),
                        Email = dataRow["Email"].ToString(),
                        Userimgpath = dataRow["Userimgpath"].ToString(),
                        PasswordHash = dataRow["PasswordHash"].ToString(),
                        PasswordSalt = dataRow["PasswordSalt"].ToString(),
                        UpdateUserId = Convert.ToInt32(dataRow["UpdateUserId"]),
                    };
                }

                // User Profile Updation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId", SqlDbType.Int) { Value = userSettings.UserId },
                    new SqlParameter("@NewUsername", SqlDbType.VarChar, 100) { Value = GetDbValue(userSettings.Username, profileUserExisting.Username) },
                    new SqlParameter("@NewFullName", SqlDbType.VarChar, 200) { Value = GetDbValue(userSettings.FullName, profileUserExisting.FullName) },
                    new SqlParameter("@NewEmailAddress", SqlDbType.VarChar, 100) { Value = GetDbValue(userSettings.Email, profileUserExisting.Email) },
                    new SqlParameter("@NewUserimgpath", SqlDbType.VarChar, 255) { Value = GetDbValue(userSettings.Userimgpath, profileUserExisting.Userimgpath) },
                    new SqlParameter("@NewPasswordHash", SqlDbType.VarChar, 255) { Value = GetDbValue(userSettings.PasswordHash, profileUserExisting.PasswordHash) },
                    new SqlParameter("@NewPasswordSalt", SqlDbType.VarChar, 255) { Value = GetDbValue(userSettings.PasswordSalt, profileUserExisting.PasswordSalt) },
                    new SqlParameter("@NewUpdateUserId", SqlDbType.Int) { Value = _currentUser.UserId }
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_UpdateUserSettings, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        NumberOfRowsAffected = (int)dataRow["RowsAffected"];
                        if (NumberOfRowsAffected < 0)
                        {
                            return MagicString;
                        }
                    }
                    else
                    {
                        return MagicString;
                    }
                }
                return profileUserExisting.Userimgpath;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while updating Settings - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating Settings.", ex);
            }
        }

        public async Task<bool> CheckUniquenessAsync(string field, string value)
        {
            try
            {
                int isUnique = 0;
                TableNameCheckUniqueness? tableEnum = null;
                if (TableNameClassForUniqueness.User.Contains(field.ToLower()))
                {
                    tableEnum = TableNameCheckUniqueness.User;
                }
                else if (TableNameClassForUniqueness.Profile.Contains(field.ToLower()))
                {
                    tableEnum = TableNameCheckUniqueness.Profile;
                }
                else if (TableNameClassForUniqueness.PageCatagory.Contains(field.ToLower()))
                {
                    tableEnum = TableNameCheckUniqueness.PageCatagory;
                }
                if (tableEnum == null)
                {
                    throw new ArgumentException("Field does not match any known table.");
                }
                // User Profile Creation
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@TableId", SqlDbType.Int) { Value = (int)tableEnum },
                    new SqlParameter("@Field", SqlDbType.VarChar, 50) { Value = field },
                    new SqlParameter("@Value", SqlDbType.VarChar, 100) { Value = value }
                };

                // Execute the command
                List<DataTable> tables = SqlDBHelper.ExecuteParameterizedNonQuery(SP_CheckUniqueness, CommandType.StoredProcedure, sqlParameters);
                if (tables.Count > 0)
                {
                    DataTable dataTable = tables[0];
                    if (dataTable.Rows.Count > 0)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        isUnique = (int)dataRow["IsUnique"];
                    }

                }
                return isUnique > 0;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error while checking existing username - SQL Exception.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while checking existing username.", ex);
            }
        }
    }
}
