USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspSearchUserByTextCriteria]    Script Date: 21-05-2024 15:30:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: To Fetch User Data from tblUsers table
-- =============================================
ALTER PROCEDURE [dbo].[uspSearchUserByTextCriteria]
-- Add the parameters for the stored procedure here
    @TextCriteria INT,
    @InputText NVARCHAR(100)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRY
		IF @TextCriteria = 1
		BEGIN 
			-- Search for User ID
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK)
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tbu.UserId like '%' + @InputText + '%'
		END

		ELSE IF @TextCriteria = 2
		BEGIN
			-- Search for National/Iqama Number
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK) 
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tbu.IdNumber like '%' + @InputText + '%'
		END

		ELSE IF @TextCriteria = 3
		BEGIN
			-- Search for Full Name
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK) 
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tbu.FullName like '%' + @InputText + '%'
		END

		ELSE IF @TextCriteria = 4
		BEGIN
			-- Search for Email Address
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK) 
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tbu.Email like '%' + @InputText + '%'
		END

		ELSE IF @TextCriteria = 5
		BEGIN
			-- Search for Mobile Number
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK) 
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tbu.MobileNo like '%' + @InputText + '%'
		END

		ELSE IF @TextCriteria = 6
		BEGIN
			-- Search for Profile Name
			SELECT tbu.UserId, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, tbu.ProfileId, tp.ProfileName, tbu.[Status] FROM tblUsers tbu WITH (NOLOCK) 
			INNER JOIN tblUserProfile tup WITH (NOLOCK) on  tbu.UserId = tup.UserId
			INNER JOIN tblProfile tp WITH (NOLOCK) on tup.ProfileId = tp.ProfileId
			WHERE tp.ProfileName like '%' + @InputText + '%'
		END

	END TRY

	BEGIN CATCH
		-- Handle exceptions
		SELECT
			ERROR_NUMBER() AS ErrorNumber
		   ,ERROR_MESSAGE() AS ErrorMessage
		   ,ERROR_SEVERITY() AS ErrorSeverity
		   ,ERROR_STATE() AS ErrorState
		   ,ERROR_LINE() AS ErrorLine
		   ,ERROR_PROCEDURE() AS ErrorProcedure;

	END CATCH;

END