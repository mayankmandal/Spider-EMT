USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddNewUserProfile]    Script Date: 21-05-2024 15:28:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert New Relationship between User & Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspAddNewUserProfile]
    -- Add the parameters for the stored procedure here
    @ProfileId INT,
	@UserId INT,
	@NewCreateUserId INT,
	@NewUpdateUserId INT
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

        -- Insert User Profile
        INSERT INTO [dbo].[AspNetUserRoles](RoleId,UserId, CreateDate, CreateUserId, UpdateDate, UpdateUserId) VALUES (@ProfileId, @UserId, GETDATE(), @NewCreateUserId, GETDATE(), @NewUpdateUserId);

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
