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
-- Description: To Fetch User Data from AspNetUsers table
-- =============================================
ALTER PROCEDURE [dbo].[uspSearchUserByTextCriteria]
-- Add the parameters for the stored procedure here
    @TextCriteria INT,
    @InputText NVARCHAR(100)
AS
BEGIN

	SET NOCOUNT ON;
	
	-- Validate parameters
    IF @TextCriteria IS NULL OR @InputText IS NULL
    BEGIN
        RAISERROR('Invalid input parameters.', 16, 1);
        RETURN;
    END

	BEGIN TRY
		-- Use a common table expression (CTE) to avoid repeating joins
        WITH UserProfile AS (
            SELECT tbu.Id, tbu.IdNumber, tbu.FullName, tbu.Email, tbu.MobileNo, 
                   tp.Id AS ProfileId, tp.[Name] AS ProfileName, tbu.Username, 
                   tbu.IsActive, tbu.IsActiveDirectoryUser, tbu.ChangePassword, tbu.Userimgpath
            FROM AspNetUsers tbu WITH (NOLOCK)
            LEFT JOIN AspNetUserRoles tup WITH (NOLOCK) ON tbu.Id = tup.UserId
            LEFT JOIN AspNetRoles tp WITH (NOLOCK) ON tup.RoleId = tp.Id
        )
		SELECT * 
        FROM UserProfile UPL
        WHERE 
            (@TextCriteria = 1 AND UPL.Id LIKE '%' + @InputText + '%')
            OR (@TextCriteria = 2 AND UPL.IdNumber LIKE '%' + @InputText + '%')
            OR (@TextCriteria = 3 AND UPL.FullName LIKE '%' + @InputText + '%')
            OR (@TextCriteria = 4 AND UPL.Email LIKE '%' + @InputText + '%')
            OR (@TextCriteria = 5 AND UPL.MobileNo LIKE '%' + @InputText + '%')
            OR (@TextCriteria = 6 AND UPL.ProfileName LIKE '%' + @InputText + '%')

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