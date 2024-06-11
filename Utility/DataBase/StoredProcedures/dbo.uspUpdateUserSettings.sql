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
	@NewUsername VARCHAR(20),
	@NewFullName VARCHAR(100),
	@NewEmailAddress VARCHAR(50),
	@NewProfilePhotoPath VARCHAR(200)
AS
BEGIN
    
    SET NOCOUNT ON;
	DECLARE @OldProfilePhotoPath VARCHAR(100);

    BEGIN TRY
        -- Check if UserId exists
        IF EXISTS (SELECT 1 FROM [tblUsers] WHERE UserId = @UserId)
        BEGIN
			-- Get the current ProfileId
            SELECT @OldProfilePhotoPath = Userimgpath FROM [tblCurrentUser] WHERE UserId = @UserId;

			UPDATE [tblUsers]
            SET FullName = @NewFullName,
                Email = @NewEmailAddress
            WHERE UserId = @UserId;

			-- Return the number of rows affected
            SELECT @@ROWCOUNT AS RowsAffectedUsers;

			IF EXISTS (SELECT 1 FROM [tblCurrentUser] WHERE UserId = @UserId)
			BEGIN
				UPDATE [tblCurrentUser]
					SET Username = @NewUsername,
						Userimgpath = @NewProfilePhotoPath
					WHERE UserId = @UserId;

				-- Return the number of rows affected
				SELECT @@ROWCOUNT AS RowsAffectedCurrentUser;
			END

			SELECT @OldProfilePhotoPath as OldProfilePhotoPath

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
