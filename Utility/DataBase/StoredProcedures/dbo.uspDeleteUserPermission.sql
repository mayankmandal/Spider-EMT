USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspDeleteUserPermission]    Script Date: 21-05-2024 15:30:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Delete User Permissions for Page and Category
-- =============================================
ALTER PROCEDURE [dbo].[uspDeleteUserPermission]
    -- Add the parameters for the stored procedure here 
    @ProfileId INT,
	@State INT
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

	-- CheckExistence
        IF @State = 1
		BEGIN
			-- Delete User Permission for User Access Control directly with Pages
			IF EXISTS (SELECT 1 FROM [dbo].[tblUserPermission] WITH (NOLOCK) WHERE ProfileId = @ProfileId AND PageCatId IS NULL AND PageId IS NOT NULL)
				BEGIN
					DELETE FROM [dbo].[tblUserPermission] WHERE ProfileId = @ProfileId AND PageCatId IS NULL AND PageId IS NOT NULL;

					SELECT @@ROWCOUNT AS RowsAffected;
				END
		END

		 -- InsertData
        ELSE IF @State = 2
        BEGIN
            -- Delete User Permission for Profiles access over Categories
			IF EXISTS (SELECT 1 FROM [dbo].[tblUserPermission] WITH (NOLOCK) WHERE ProfileId = @ProfileId AND PageId IS NULL AND PageCatId IS NOT NULL)
				BEGIN
					DELETE FROM [dbo].[tblUserPermission] WHERE ProfileId = @ProfileId AND PageId IS NULL AND PageCatId IS NOT NULL;

					SELECT @@ROWCOUNT AS RowsAffected;
				END
        END

        ELSE
			BEGIN
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;

				-- ProfileId does not exist, return an error code
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

        -- Return an error code
        SELECT -1 AS RowsAffected;
    END CATCH;

END
