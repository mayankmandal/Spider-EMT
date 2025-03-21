USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddUserPermission]    Script Date: 21-05-2024 15:29:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert User Permissions for Page and Category
-- =============================================
ALTER PROCEDURE [dbo].[uspAddUserPermission]
    -- Add the parameters for the stored procedure here
    @NewProfileId INT,
	@NewPageId INT,
	@NewPageCatId INT,
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

        -- Insert User Permission
        INSERT INTO [dbo].[tblUserPermission](ProfileId, PageId, PageCatId, CreateDate, CreateUserId, UpdateDate, UpdateUserId) VALUES (@NewProfileId, @NewPageId, @NewPageCatId, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId);

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

END
