USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddNewUser]    Script Date: 22-05-2024 17:35:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert User Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspAddNewUser]
    -- Add the parameters for the stored procedure here
	@NewIdNumber VARCHAR(20),
	@NewFullName VARCHAR(100),
	@NewEmailAddress VARCHAR(50),
	@NewMobileNumber VARCHAR(15),
	@NewProfileId INT,
	@NewUsername VARCHAR(100),
	@NewUserimgpath VARCHAR(255),
	@NewPasswordHash VARCHAR(255),
	@NewIsActive BIT,
	@NewIsActiveDirectoryUser BIT,
	@NewChangePassword BIT,
	@NewLastLoginActivity DATETIME,
	@NewCreateUserId INT,
	@NewUpdateUserId INT

AS
BEGIN
    
    SET NOCOUNT ON;

	DECLARE @NewUserId INT = 0; -- Declare variable to store ProfileId

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

        -- Insert User Profile
        INSERT INTO [dbo].[AspNetUsers](IdNumber,FullName,Email,MobileNo,Id, Username, Userimgpath, PasswordHash, IsActive, IsActiveDirectoryUser, ChangePassword, LastLoginActivity, CreateDate, CreateUserId, UpdateDate, UpdateUserId) 
		VALUES (@NewIdNumber,@NewFullName,@NewEmailAddress,@NewMobileNumber,@NewProfileId, @NewUsername, @NewUserimgpath, @NewPasswordHash, @NewIsActive, @NewIsActiveDirectoryUser, @NewChangePassword, @NewLastLoginActivity, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId);
		-- Retrieve the newly generated ProfileId
		SET @NewUserId = SCOPE_IDENTITY(); -- Get the last inserted identity value

		IF @NewUserId <> 0
		BEGIN
			EXEC [dbo].[uspAddNewUserProfile] @NewProfileId, @NewUserId, @NewCreateUserId, @NewUpdateUserId
		END

		-- If everything is successful, commit the transaction
        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH
		-- If an error occurs, rollback the transaction
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

		-- Set the output parameter to NULL in case of error
        SET @NewUserId = NULL;

    END CATCH;

	-- Check if @NewProfileId is initialized and return the appropriate message
	IF @NewUserId <> 0
	BEGIN
		-- If @NewProfileId is not Null , return the ProfileId
		SELECT @NewUserId AS UserId;
	END
	ELSE
	BEGIN
		-- If NewProfileId is NULL, return error
		SELECT 'Error' AS ErrorMessage;
	END
END
