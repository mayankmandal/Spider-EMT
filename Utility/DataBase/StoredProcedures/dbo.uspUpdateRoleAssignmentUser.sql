USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspUpdateRoleAssignmentUser]    Script Date: 30-07-2024 15:31:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update Flag for User's Role Assignment State
-- =============================================
ALTER PROCEDURE [dbo].[uspUpdateRoleAssignmentUser]
    -- Add the parameters for the stored procedure here
    @UserId INT = 0,
	@NewRoleAssignmentEnabled BIT = NULL,
	@NewUpdateUserId INT = 0
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
		-- Start transaction
        BEGIN TRANSACTION;

        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [AspNetUsers] WITH (NOLOCK) WHERE Id = @UserId)
			BEGIN
				UPDATE AspNetUsers SET RoleAssignmentEnabled = @NewRoleAssignmentEnabled, 
				UpdateDate = GETDATE(),
				UpdateUserId = @NewUpdateUserId
				WHERE Id = @UserId

				-- Commit transaction
				COMMIT TRANSACTION;

				-- Return the number of rows affected
				SELECT @@ROWCOUNT AS RowsAffected;
			END
		ELSE
        BEGIN
			-- UserId does not exist
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            -- UserId does not exist
            SELECT -1 AS RowsAffected;
        END
    END TRY
    BEGIN CATCH
		-- Rollback transaction if there is an error
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
