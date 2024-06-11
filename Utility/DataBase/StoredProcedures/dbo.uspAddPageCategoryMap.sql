USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddPageCategoryMap]    Script Date: 21-05-2024 15:28:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert New Category in tblPageCategory table
-- =============================================
ALTER PROCEDURE [dbo].[uspAddPageCategoryMap]
    -- Add the parameters for the stored procedure here
    @NewPageCatId INT,
    @NewPageId INT,
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if both @NewPageCatId and @NewPageId are not null
        IF @NewPageCatId IS NOT NULL AND @NewPageId IS NOT NULL
        BEGIN
			INSERT INTO tblPageCategoryMap (PageId, PageCatId, CreateDate, CreateUserId, UpdateDate, UpdateUserId)
			VALUES (@NewPageId, @NewPageCatId, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId)
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

    END CATCH;

END
