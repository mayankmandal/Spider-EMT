USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddNewUser]    Script Date: 21-05-2024 15:29:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Insert User Profile
-- =============================================
ALTER PROCEDURE [dbo].[uspAddNewUser]
    -- Add the parameters for the stored procedure here
	@NewIdNumber VARCHAR(20),
	@NewFullName VARCHAR(100),
	@NewEmailAddress VARCHAR(50),
	@NewMobileNumber VARCHAR(15),
	@NewProfileId INT,
	@NewUserStatus VARCHAR(20)

AS
BEGIN
    
    SET NOCOUNT ON;

	DECLARE @UserId INT; -- Declare variable to store ProfileId

    BEGIN TRY
        -- Insert User Profile
        INSERT INTO [dbo].[tblUsers](IdNumber,FullName,Email,MobileNo,ProfileId,[Status]) 
		VALUES (@NewIdNumber,@NewFullName,@NewEmailAddress,@NewMobileNumber,@NewProfileId,@NewUserStatus);
		-- Retrieve the newly generated ProfileId
		SET @UserId = SCOPE_IDENTITY(); -- Get the last inserted identity value

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
        SET @UserId = NULL;

    END CATCH;

	-- Check if @NewProfileId is initialized and return the appropriate message
	IF @UserId IS NOT NULL
	BEGIN
		-- If @NewProfileId is not Null , return the ProfileId
		SELECT @UserId AS UserId;
	END
	ELSE
	BEGIN
		-- If NewProfileId is NULL, return error
		SELECT 'Error' AS ErrorMessage;
	END
END
