USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddUserProfile]   Script Date: 10-04-2024 11:41:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert User Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspAddUserProfile]
    -- Add the parameters for the stored procedure here
    @NewProfileName VARCHAR(50)	
AS
BEGIN
    
    SET NOCOUNT ON;

	DECLARE @NewProfileId INT; -- Declare variable to store ProfileId

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

        -- Insert User Profile
        INSERT INTO [dbo].[AspNetRoles] VALUES (@NewProfileName);
		-- Retrieve the newly generated ProfileId
		SET @NewProfileId = SCOPE_IDENTITY(); -- Get the last inserted identity value

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
        SET @NewProfileId = NULL;

    END CATCH;

	-- Check if @NewProfileId is initialized and return the appropriate message
	IF @NewProfileId IS NOT NULL
	BEGIN
		-- If @NewProfileId is not Null , return the ProfileId
		SELECT @NewProfileId AS ProfileId;
	END
	ELSE
	BEGIN
		-- If NewProfileId is NULL, return error
		SELECT 'Error' AS ErrorMessage;
	END
END
