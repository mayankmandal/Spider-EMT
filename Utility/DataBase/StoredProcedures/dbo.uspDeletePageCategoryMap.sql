USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspDeletePageCategoryMap]    Script Date: 21-05-2024 15:30:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: DELETE relationship between Categories and Pages
-- =============================================
ALTER PROCEDURE [dbo].[uspDeletePageCategoryMap]
    -- Add the parameters for the stored procedure here 
	@State NVARCHAR(3) = NULL,
    @PageId INT = NULL,
    @PageCatId INT = NULL
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY

		-- Start the transaction
        BEGIN TRANSACTION;

        -- CheckExistence
        IF @State = 'PI'
        BEGIN
			-- Check if a row exists and perform delete operation
            IF EXISTS (SELECT 1 FROM tblPageCategoryMap WITH (NOLOCK) WHERE PageId = @PageId)
            BEGIN
                DELETE FROM tblPageCategoryMap
				WHERE PageId = @PageId
            END
        END

        -- InsertData
        ELSE IF @State = 'PC'
        BEGIN
            -- Check if a row exists and perform delete operation
            IF EXISTS (SELECT 1 FROM tblPageCategoryMap WITH (NOLOCK) WHERE PageCatId = @PageCatId)
            BEGIN
                DELETE FROM tblPageCategoryMap
				WHERE PageCatId = @PageCatId
            END
        END

        -- UpdateData
        ELSE IF @State = 'BP'
        BEGIN
            -- Check if a row exists and perform delete operation
            IF EXISTS (SELECT 1 FROM tblPageCategoryMap WITH (NOLOCK) WHERE PageId = @PageId and PageCatId = @PageCatId)
            BEGIN
                DELETE FROM tblPageCategoryMap
				WHERE PageId = @PageId and PageCatId = @PageCatId
            END
        END

        -- DeleteData
        ELSE
        BEGIN
            -- Check if a row exists and perform delete operation
            SELECT -1 AS RowsAffected;
        END

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

    END CATCH;

END
