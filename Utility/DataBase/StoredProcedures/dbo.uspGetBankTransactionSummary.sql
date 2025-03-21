USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspGetBankTransactionSummary]    Script Date: 08-01-2024 17:25:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update transaction fees for a specific transaction
-- =============================================
CREATE PROCEDURE [dbo].[uspGetBankTransactionSummary]
    -- Add the parameters for the stored procedure here
    @StartDate DATETIME,
    @EndDate DATETIME,
    @BankId INT = NULL
AS

BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY

	-- Declare variables
    DECLARE @BankLogoFolderPath NVARCHAR(MAX) = '/images/bank_logos/';

    -- Retrieve data from the required tables
    WITH TransactionData AS (
        SELECT
            atm.TermId,
            atm.TxnDate,
            atm.TotalCWCount,
            atm.TotalBICount,
            atm.TotalMSCount
        FROM
            tblAllAtmTxn atm
        INNER JOIN
            tblRefBanks bank ON atm.BankId = bank.BankId
        WHERE
            (@BankId IS NULL OR bank.BankId = @BankId)
            AND atm.TxnDate BETWEEN @StartDate AND @EndDate
    )
    SELECT
        bank.BankNameEn AS BankNameEn,
        -- Assuming logos are in PNG format
        CONCAT(@BankLogoFolderPath, bank.BankShortName, '.png') AS BankLogoPath,
        td.TermId,
        bank.RegionEn AS RegionEn,
        bank.CityEn AS CityEn,
        td.TxnDate,
        td.TotalCWCount,
        td.TotalCWCount * tf.CWTxnFee AS TotalCWFeeAmount,
        td.TotalBICount,
        td.TotalMSCount,
        (td.TotalBICount * tf.BITxnFee) + (td.TotalMSCount * tf.MSTxnFee) AS TotalBI_MSFeeAmount,
        td.TotalCWCount + td.TotalBICount + td.TotalMSCount AS TotalTxnOnUsCount,
        (td.TotalCWCount * tf.CWTxnFee) + ((td.TotalBICount * tf.BITxnFee) + (td.TotalMSCount * tf.MSTxnFee)) AS TotalPayedAmount
    FROM
        TransactionData td
    INNER JOIN
        tblRefBanks bank ON td.TermId = bank.TermId
    INNER JOIN
        tblRefTxnFee tf ON 1=1;


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

    END CATCH;

END
