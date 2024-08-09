USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddNewProfile]    Script Date: 21-05-2024 15:28:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Add New Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspAddNewProfile]
    -- Add the parameters for the stored procedure here
    @NewProfileName VARCHAR(100),
	@NewCreateUserId INT,
	@NewUpdateUserId INT,
	@UserIdentity INT OUTPUT -- Output parameter to return the new ProfileId
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check is same name Profile already exists
        IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WITH (NOLOCK) WHERE [Name] = @NewProfileName)
			BEGIN
				-- Insert Profile
				INSERT INTO [dbo].[AspNetRoles] ([Name], NormalizedName, CreateDate, CreateUserId, UpdateDate, UpdateUserId)
				VALUES (@NewProfileName, @NewProfileName, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId)

				-- Retrieve the newly generated ProfileId
				SET @UserIdentity = SCOPE_IDENTITY(); -- Get the last inserted identity value
			END
        ELSE
        BEGIN
            -- ProfileId does not exist, return an error code
            SELECT -1 AS RowsAffected; -- Indicate that the profile already exists
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

        -- Set the output parameter to NULL in case of error
        SET @UserIdentity = NULL;
    END CATCH;
END
