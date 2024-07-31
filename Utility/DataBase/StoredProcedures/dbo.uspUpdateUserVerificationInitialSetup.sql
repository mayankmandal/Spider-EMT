USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspUpdateUserVerificationInitialSetup]    Script Date: 25-07-2024 17:38:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update User Verification Details at Initial Setup
-- =============================================
ALTER PROCEDURE [dbo].[uspUpdateUserVerificationInitialSetup]
    -- Add the parameters for the stored procedure here
    @UserId INT,
	@NewIdNumber VARCHAR(10) = NULL,
	@NewFullName VARCHAR(200) = NULL,
	@NewMobileNumber VARCHAR(15) = NULL,
	@NewUsername VARCHAR(100) = NULL,
	@NewUserimgpath VARCHAR(255) = NULL,
	@NewUpdateUserId INT = 0,
	@NewCreateUserId INT = 0
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [AspNetUsers] WHERE Id = @UserId)
        BEGIN
			-- Construct the dynamic update query
			DECLARE @sqlUpdate NVARCHAR(MAX);

			SET @sqlUpdate = 'UPDATE [AspNetUsers] SET ';

			-- Append each field if the corresponding parameter is NOT NULL

			IF @NewIdNumber IS NOT NULL
                SET @sqlUpdate += 'IdNumber = @NewIdNumber, ';
            IF @NewFullName IS NOT NULL
                SET @sqlUpdate += 'FullName = @NewFullName, ';
            IF @NewMobileNumber IS NOT NULL
                SET @sqlUpdate += 'MobileNo = @NewMobileNumber, ';
            IF @NewUsername IS NOT NULL
                SET @sqlUpdate += 'Username = @NewUsername, ';
            IF @NewUpdateUserId IS NOT NULL
                SET @sqlUpdate += 'UpdateUserId = @NewUpdateUserId, ';
			IF @NewCreateUserId IS NOT NULL
                SET @sqlUpdate += 'CreateUserId = @NewCreateUserId, ';
            IF @NewUserimgpath IS NOT NULL
                SET @sqlUpdate += 'Userimgpath = @NewUserimgpath, ';

			SET @sqlUpdate += 'UpdateDate = GETDATE(), CreateDate = GETDATE(), ';
			-- Remove the trailing comma and space
			SET @sqlUpdate = LEFT(@sqlUpdate, LEN(@sqlUpdate) - 1);

			-- Add WHERE clause to restrict update to specific UserId
			SET @sqlUpdate += ' WHERE Id = @UserId';

			-- Execute the update query
            EXEC sp_executesql @sqlUpdate,
                N'@NewIdNumber VARCHAR(10), @NewFullName VARCHAR(200), @NewMobileNumber VARCHAR(15), 
                  @NewUsername VARCHAR(100), @NewCreateUserId INT, @NewUpdateUserId INT, @NewUserimgpath VARCHAR(255), 
				  @UserId INT',
                @NewIdNumber, @NewFullName, @NewMobileNumber, 
                @NewUsername, @NewCreateUserId, @NewUpdateUserId, @NewUserimgpath,
				@UserId;

            ---- Return the number of rows affected
            --SELECT @@ROWCOUNT AS RowsAffected;

			IF @@rowcount = 1
			BEGIN
				IF EXISTS(
					SELECT 1 FROM [AspNetUsers]
					WHERE 
						ID = @UserId AND
						EmailConfirmed = 1 AND
						TwoFactorEnabled = 1 AND
						UserName IS NOT NULL AND
						MobileNo IS NOT NULL AND
						Email IS NOT NULL AND
						IdNumber IS NOT NULL
				)
				BEGIN
					-- Update UserVerificationSetupEnabled to 1
					UPDATE [AspNetUsers]
					SET UserVerificationSetupEnabled = 1
					WHERE Id = @UserId;

					-- Return the number of rows affected
					SELECT @@rowcount AS RowsAffected;
				END
				ELSE
				BEGIN
					-- Condition not met, return -1
					SELECT -1 AS RowsAffected;
				END
			END
			ELSE
			BEGIN
				-- No rows affected, return -1
				SELECT -1 AS RowsAffected;
			END
		END
		ELSE
        BEGIN
            -- UserId does not exist
            SELECT -1 AS RowsAffected;
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

        -- Return an error code
        SELECT -1 AS RowsAffected;
    END CATCH;
END
