USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspUpdateUser]    Script Date: 21-05-2024 15:31:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update User Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspUpdateUser]
    -- Add the parameters for the stored procedure here
    @UserId INT,
	@NewIdNumber VARCHAR(10) = NULL,
	@NewFullName VARCHAR(200) = NULL,
	@NewEmailAddress VARCHAR(100) = NULL,
	@NewMobileNumber VARCHAR(15) = NULL,
	@NewProfileId INT = NULL,
	@NewUsername VARCHAR(100) = NULL,
	@NewUserimgpath VARCHAR(255) = NULL,
	@NewIsActive BIT = NULL,
	@NewIsActiveDirectoryUser BIT = NULL,
	@NewChangePassword BIT = NULL,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @OldProfileId INT;
	-- Construct the dynamic update query
	DECLARE @sqlUpdate NVARCHAR(MAX);

    BEGIN TRY
		-- Start transaction
        BEGIN TRANSACTION;

        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [AspNetUsers] WITH (NOLOCK) WHERE Id = @UserId)
        BEGIN

			-- Get the current ProfileId
            SELECT @OldProfileId = anur.RoleId FROM [AspNetUsers] u with (NOLOCK) INNER JOIN AspNetUserRoles anur WITH (NOLOCK) ON u.Id = anur.UserId WHERE u.Id = @UserId;

			-- Convert NULL to a default value
			SET @OldProfileId = ISNULL(@OldProfileId, 0);
			SET @NewProfileId = ISNULL(@NewProfileId, 0);

			-- Check if the ProfileId needs to be updated
            IF @OldProfileId <> @NewProfileId
			BEGIN

				-- Disabled RoleAssignment for User since profile getting unassigned
				EXEC [dbo].[uspUpdateRoleAssignmentUser] @UserId, 0, @NewUpdateUserId

                -- Update the profile using the existing stored procedures
                EXEC [dbo].[uspDeleteUserProfile] @UserId;
                EXEC [dbo].[uspAddNewUserProfile] @NewProfileId, @UserId, @NewUpdateUserId, @NewUpdateUserId;

				-- Enabled RoleAssignment for User since profile is assigned
				EXEC [dbo].[uspUpdateRoleAssignmentUser] @UserId, 1, @NewUpdateUserId
            END

			SET @sqlUpdate = 'UPDATE [AspNetUsers] SET ';

			-- Append each field if the corresponding parameter is NOT NULL

			IF @NewIdNumber IS NOT NULL
                SET @sqlUpdate += 'IdNumber = @NewIdNumber, ';
            IF @NewFullName IS NOT NULL
                SET @sqlUpdate += 'FullName = @NewFullName, ';
            IF @NewEmailAddress IS NOT NULL
                SET @sqlUpdate += 'Email = @NewEmailAddress, ';
            IF @NewMobileNumber IS NOT NULL
                SET @sqlUpdate += 'MobileNo = @NewMobileNumber, ';
            IF @NewUsername IS NOT NULL
                SET @sqlUpdate += 'Username = @NewUsername, ';
            IF @NewUpdateUserId IS NOT NULL
                SET @sqlUpdate += 'UpdateUserId = @NewUpdateUserId, ';
            IF @NewUserimgpath IS NOT NULL
                SET @sqlUpdate += 'Userimgpath = @NewUserimgpath, ';
            IF @NewIsActive IS NOT NULL
                SET @sqlUpdate += 'IsActive = @NewIsActive, ';
            IF @NewIsActiveDirectoryUser IS NOT NULL
                SET @sqlUpdate += 'IsActiveDirectoryUser = @NewIsActiveDirectoryUser, ';
            IF @NewChangePassword IS NOT NULL
                SET @sqlUpdate += 'ChangePassword = @NewChangePassword, ';

			SET @sqlUpdate += 'UpdateDate = GETDATE(), ';
			-- Remove the trailing comma and space
			SET @sqlUpdate = LEFT(@sqlUpdate, LEN(@sqlUpdate) - 1);

			-- Add WHERE clause to restrict update to specific UserId
			SET @sqlUpdate += ' WHERE Id = @UserId';

			-- Execute the update query
            EXEC sp_executesql @sqlUpdate,
                N'@NewIdNumber VARCHAR(10), @NewFullName VARCHAR(200), @NewEmailAddress VARCHAR(100), @NewMobileNumber VARCHAR(15), 
                  @NewUsername VARCHAR(100), @NewUpdateUserId INT, @NewUserimgpath VARCHAR(255), 
				  @NewIsActive BIT, @NewIsActiveDirectoryUser BIT, @NewChangePassword BIT, @UserId INT',
                @NewIdNumber, @NewFullName, @NewEmailAddress, @NewMobileNumber, 
                @NewUsername, @NewUpdateUserId, @NewUserimgpath,
				@NewIsActive, @NewIsActiveDirectoryUser, @NewChangePassword, @UserId;

			-- Commit transaction
            COMMIT TRANSACTION;

            -- Return the number of rows affected
            SELECT @@ROWCOUNT AS RowsAffected;
			
		END
		ELSE
        BEGIN
		-- UserId does not exist
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            SELECT -1 AS RowsAffected;
        END
    END TRY
    BEGIN CATCH
		-- Rollback transaction if there is an error
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Handle exceptions
        SELECT
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_MESSAGE() AS ErrorMessage,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState,
            ERROR_LINE() AS ErrorLine,
            ERROR_PROCEDURE() AS ErrorProcedure;

        -- Return an error code
        SELECT -1 AS RowsAffected;
    END CATCH;
END
