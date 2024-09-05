USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspBaseUserRole]    Script Date: 08-08-2024 19:39:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: For creating New Base Role to every User at Initial Point
-- =============================================
ALTER PROCEDURE [dbo].[uspBaseUserRole]
    -- Add the parameters for the stored procedure here
    @NewProfileName VARCHAR(100),
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @RoleIdentity INT;

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

		-- Execute the stored procedure and capture the output
		EXEC [dbo].[uspAddNewProfile] 
			@NewProfileName = @NewProfileName, 
            @NewCreateUserId = @NewCreateUserId, 
            @NewUpdateUserId = @NewUpdateUserId,
            @UserIdentity = @RoleIdentity OUTPUT;

		-- Check if the profile was created successfully
		IF @RoleIdentity IS NULL OR @RoleIdentity = -1
		BEGIN
			-- Handle error case if the profile already exists or if there was an error
			SELECT 'Error' AS ErrorMessage;
			RETURN -1;
		END

         -- Insert records into tblUserPermission for the given pages
        INSERT INTO tblUserPermission (ProfileId, PageId, PageCatId, CreateDate, CreateUserId, UpdateDate, UpdateUserId)
		 SELECT 
            @RoleIdentity, 
            PageId, 
            NULL, 
            GETDATE(), 
            @NewCreateUserId, 
            GETDATE(), 
            @NewUpdateUserId
        FROM tblPage
        WHERE PageUrl IN (
            '/Account/AccessDenied',
			'/Account/AuthenticatorWithMFASetup',
			'/Account/ConfirmEmail',
			'/Account/Login',
			'/Account/LoginTwoFactorWithAuthenticator',
			'/Account/Logout',
			'/Account/Register',
			'/Account/UserRoleAssignment',
			'/Account/UserVerificationSetup',
			'/Dashboard',
			'/ReadUserProfile',
			'/EditSettings',
			'/Error'
        );

        -- If everything is successful, commit the transaction
        COMMIT TRANSACTION;

		SELECT 'Success' AS Message, @RoleIdentity AS RoleId;

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

    END CATCH;

END
