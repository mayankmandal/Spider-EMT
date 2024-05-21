USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspUpdateTransactionFees]    Script Date: 21-05-2024 15:31:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update transaction fees for a specific transaction
-- =============================================
ALTER PROCEDURE [dbo].[uspUpdateTransactionFees]
    -- Add the parameters for the stored procedure here
    @TransactionFeeId INT,
    @NewCWTxnFee DECIMAL(5,2),
    @NewBITxnFee DECIMAL(5,2),
    @NewMSTxnFee DECIMAL(5,2)
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
        -- Update transaction fees
        UPDATE [dbo].[tblRefTxnFee]
        SET
            [CWTxnFee] = @NewCWTxnFee,
            [BITxnFee] = @NewBITxnFee,
            [MSTxnFee] = @NewMSTxnFee
        WHERE
            [TxnFeeId] = @TransactionFeeId;

        -- Select the updated values for verification
        SELECT
            [TxnFeeId],
            [CWTxnFee],
            [BITxnFee],
            [MSTxnFee]
        FROM
            [dbo].[tblRefTxnFee]
        WHERE
            [TxnFeeId] = @TransactionFeeId;
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
