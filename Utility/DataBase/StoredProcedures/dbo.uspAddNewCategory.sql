USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddNewCategory]    Script Date: 21-05-2024 15:27:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert New Category in tblPageCategory table
-- =============================================
ALTER PROCEDURE [dbo].[uspAddNewCategory]
    -- Add the parameters for the stored procedure here
    @NewCategoryName Varchar(50),
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;

	DECLARE @UserIdentity INT; -- Declare variable to store ProfileId
	
    BEGIN TRY

		-- Start the transaction
        BEGIN TRANSACTION;

        -- Insert User Profile
        INSERT INTO [dbo].[tblPageCatagory](CatagoryName, CreateDate, CreateUserId, UpdateDate, UpdateUserId) VALUES (@NewCategoryName, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId);
		-- Retrieve the newly generated ProfileId
		SET @UserIdentity = SCOPE_IDENTITY(); -- Get the last inserted identity value

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
