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
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @UserIdentity INT; -- Declare variable to store ProfileId

    BEGIN TRY
        -- Check is same name Profile already exists
        IF NOT EXISTS (SELECT 1 FROM [dbo].[tblProfile] WITH (NOLOCK) WHERE ProfileName = @NewProfileName)
			BEGIN
				-- Insert Profile
				INSERT INTO [dbo].[tblProfile] (ProfileName, CreateDate, CreateUserId, UpdateDate, UpdateUserId)
				VALUES (@NewProfileName, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId)
				-- Retrieve the newly generated ProfileId
				SET @UserIdentity = SCOPE_IDENTITY(); -- Get the last inserted identity value
			END
        ELSE
        BEGIN
            -- ProfileId does not exist, return an error code
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

        -- Set the output parameter to NULL in case of error
        SET @UserIdentity = NULL;
    END CATCH;
	-- Check if @NewCategoryId is initialized and return the appropriate message
	IF @UserIdentity IS NOT NULL
	BEGIN
		-- If @NewCategoryId is not Null , return the ProfileId
		SELECT @UserIdentity AS UserIdentity;
	END
	ELSE
	BEGIN
		-- If NewCategoryId is NULL, return error
		SELECT 'Error' AS ErrorMessage;
	END
END
