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
	@NewIdNumber VARCHAR(20),
	@NewFullName VARCHAR(100),
	@NewEmailAddress VARCHAR(50),
	@NewMobileNumber VARCHAR(15),
	@NewProfileId INT,
	@NewUsername VARCHAR(20),
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @OldProfileId INT;

    BEGIN TRY
        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [tblUsers] WHERE UserId = @UserId)
        BEGIN
			-- Get the current ProfileId
            SELECT @OldProfileId = ProfileId FROM [tblUsers] WHERE UserId = @UserId;

			-- Check if the ProfileId needs to be updated
            IF @OldProfileId <> @NewProfileId
			BEGIN
                -- Update the profile using the existing stored procedures
                EXEC [dbo].[uspDeleteUserProfile] @UserId;
                EXEC [dbo].[uspAddNewUserProfile] @NewProfileId, @UserId, @NewCreateUserId, @NewUpdateUserId;
            END

			-- Update the user details
            UPDATE [tblUsers]
            SET IdNumber = @NewIdNumber,
                FullName = @NewFullName,
                Email = @NewEmailAddress,
                MobileNo = @NewMobileNumber,
                ProfileId = @NewProfileId,
                [Status] = @NewUserStatus
            WHERE UserId = @UserId;
			
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
