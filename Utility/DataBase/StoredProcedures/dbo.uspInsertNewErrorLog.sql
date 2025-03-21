USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspInsertNewErrorLog]   Script Date: 11-07-2024 14:44:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert New Error Log
-- =============================================
ALTER PROCEDURE [dbo].[uspInsertNewErrorLog]
    -- Add the parameters for the stored procedure here
	@NewErrorMessage VARCHAR(MAX),
	@NewStackTrace VARCHAR(MAX),
	@NewCreateUserId INT,
	@NewUpdateUserId INT

AS
BEGIN
    
    SET NOCOUNT ON;

	DECLARE @NewLogId INT = 0; -- Declare variable to store ProfileId

    BEGIN TRY
		-- Start a transaction
        BEGIN TRANSACTION;

		INSERT INTO tblDBErrorLog (ErrorMessage, StackTrace, CreateUserId, UpdateDate, UpdateUserId)
                      VALUES (@NewErrorMessage, @NewStackTrace, @NewCreateUserId, GETDATE(), @NewUpdateUserId);
        SET @NewLogId = SCOPE_IDENTITY(); -- Get the last inserted identity value

		-- Commit the transaction
        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
		-- Rollback the transaction in case of an error
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
        SET @NewLogId = NULL;

    END CATCH;

	-- Check if @NewProfileId is initialized and return the appropriate message
	IF @NewLogId <> 0
	BEGIN
		-- If @NewProfileId is not Null , return the ProfileId
		SELECT @NewLogId AS LogId;
	END
	ELSE
	BEGIN
		-- If NewProfileId is NULL, return error
		SELECT 'Error occurred while inserting the new error log.' AS ErrorMessage;
	END
END
