USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspUpdateUserSettings]    Script Date: 07-06-2024 11:03:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update User Settings
-- =============================================
ALTER PROCEDURE [dbo].[uspUpdateUserSettings]
    -- Add the parameters for the stored procedure here
    @UserId INT,
	@NewUsername VARCHAR(100) = NULL,
	@NewFullName VARCHAR(200) = NULL,
	@NewEmailAddress VARCHAR(100) = NULL,
	@NewUserimgpath VARCHAR(255) = NULL,
	@NewPasswordHash VARCHAR(255) = NULL,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @OldProfilePhotoPath VARCHAR(100);

    BEGIN TRY
		-- Start transaction
        BEGIN TRANSACTION;

        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [AspNetUsers] WITH (NOLOCK) WHERE Id = @UserId)
        BEGIN
			-- Construct the dynamic update query
			DECLARE @sqlUpdate NVARCHAR(MAX);

			SET @sqlUpdate = 'UPDATE [AspNetUsers] SET ';

			-- Append each field if the corresponding parameter is NOT NULL
			IF @NewUsername IS NOT NULL
                SET @sqlUpdate += 'Username = @NewUsername, ';
            IF @NewFullName IS NOT NULL
                SET @sqlUpdate += 'FullName = @NewFullName, ';
            IF @NewEmailAddress IS NOT NULL
                SET @sqlUpdate += 'Email = @NewEmailAddress, ';
            IF @NewUserimgpath IS NOT NULL
                SET @sqlUpdate += 'Userimgpath = @NewUserimgpath, ';
			IF @NewPasswordHash IS NOT NULL
                SET @sqlUpdate += 'PasswordHash = @NewPasswordHash, ';
            IF @NewUpdateUserId IS NOT NULL
                SET @sqlUpdate += 'UpdateUserId = @NewUpdateUserId, ';

			SET @sqlUpdate += 'UpdateDate = GETDATE(), ';
			-- Remove the trailing comma and space
			SET @sqlUpdate = LEFT(@sqlUpdate, LEN(@sqlUpdate) - 1);

			-- Add WHERE clause to restrict update to specific UserId
			SET @sqlUpdate += ' WHERE Id = @UserId';

			-- Execute the update query
            EXEC sp_executesql @sqlUpdate,
                N'@NewUsername VARCHAR(100), @NewFullName VARCHAR(200), @NewEmailAddress VARCHAR(100),@NewUserimgpath VARCHAR(255),
				@NewPasswordHash VARCHAR(255), @NewUpdateUserId INT, @UserId INT',
                @NewUsername, @NewFullName, @NewEmailAddress, @NewUserimgpath, 
				@NewPasswordHash, @NewUpdateUserId, @UserId;

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
