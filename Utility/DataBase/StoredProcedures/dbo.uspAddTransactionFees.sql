USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspAddTransactionFees]    Script Date: 21-05-2024 15:29:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Update transaction fees for a specific transaction
-- =============================================
ALTER PROCEDURE [dbo].[uspAddTransactionFees]
    -- Add the parameters for the stored procedure here
    @NewCWTxnFee DECIMAL(5,2),
    @NewBITxnFee DECIMAL(5,2),
    @NewMSTxnFee DECIMAL(5,2)
AS
BEGIN
    
    SET NOCOUNT ON;

    BEGIN TRY
		-- Start the transaction
        BEGIN TRANSACTION;

        -- Update transaction fees
        INSERT INTO [dbo].[tblRefTxnFee]([CWTxnFee], [BITxnFee], [MSTxnFee]) VALUES (@NewCWTxnFee, @NewBITxnFee, @NewMSTxnFee)

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
