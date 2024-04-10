USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddUserPermission]    Script Date: 10-04-2024 15:01:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert User Permissions for Page and Category
-- =============================================
CREATE PROCEDURE [dbo].[uspAddUserPermission]
    -- Add the parameters for the stored procedure here
    @NewProfileId INT,
	@NewPageId INT,
	@NewPageCatId INT
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
        -- Insert User Permission
        INSERT INTO [dbo].[tblUserPermission](ProfileId, PageId, PageCatId) VALUES (@NewProfileId, @NewPageId, @NewPageCatId);
		
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
        SET @NewProfileId = NULL;

    END CATCH;

END
