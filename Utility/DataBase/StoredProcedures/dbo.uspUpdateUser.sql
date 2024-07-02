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

    BEGIN TRY
        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [tblUsers] WHERE UserId = @UserId)
        BEGIN
			-- Construct the dynamic update query
			DECLARE @sqlUpdate NVARCHAR(MAX);

			-- Get the current ProfileId
            SELECT @OldProfileId = ProfileId FROM [tblUsers] with (NOLOCK) WHERE UserId = @UserId;

			-- Check if the ProfileId needs to be updated
            IF @OldProfileId <> @NewProfileId
			BEGIN
                -- Update the profile using the existing stored procedures
                EXEC [dbo].[uspDeleteUserProfile] @UserId;
                EXEC [dbo].[uspAddNewUserProfile] @NewProfileId, @UserId, @NewUpdateUserId, @NewUpdateUserId;
            END

			SET @sqlUpdate = 'UPDATE [tblUsers] SET ';

			-- Append each field if the corresponding parameter is NOT NULL

			IF @NewIdNumber IS NOT NULL
                SET @sqlUpdate += 'IdNumber = @NewIdNumber, ';
            IF @NewFullName IS NOT NULL
                SET @sqlUpdate += 'FullName = @NewFullName, ';
            IF @NewEmailAddress IS NOT NULL
                SET @sqlUpdate += 'Email = @NewEmailAddress, ';
            IF @NewMobileNumber IS NOT NULL
                SET @sqlUpdate += 'MobileNo = @NewMobileNumber, ';
            IF @NewProfileId IS NOT NULL
                SET @sqlUpdate += 'ProfileId = @NewProfileId, ';
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
			SET @sqlUpdate += ' WHERE UserId = @UserId';

			-- Execute the update query
            EXEC sp_executesql @sqlUpdate,
                N'@NewIdNumber VARCHAR(10), @NewFullName VARCHAR(200), @NewEmailAddress VARCHAR(100), @NewMobileNumber VARCHAR(15), 
                  @NewProfileId INT, @NewUsername VARCHAR(100), @NewUpdateUserId INT, @NewUserimgpath VARCHAR(255), 
				  @NewIsActive BIT, @NewIsActiveDirectoryUser BIT, @NewChangePassword BIT, @UserId INT',
                @NewIdNumber, @NewFullName, @NewEmailAddress, @NewMobileNumber, 
                @NewProfileId, @NewUsername, @NewUpdateUserId, @NewUserimgpath,
				@NewIsActive, @NewIsActiveDirectoryUser, @NewChangePassword, @UserId;

            -- Return the number of rows affected
            SELECT @@ROWCOUNT AS RowsAffected;
			
		END
		ELSE
        BEGIN
            -- UserId does not exist
            SELECT -1 AS RowsAffected;
        END
    END TRY
    BEGIN CATCH
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
