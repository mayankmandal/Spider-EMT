USE [SpiderETMDB]
GO
/****** Object:  StoredProcedure [dbo].[uspTopicTrace]    Script Date: 21-05-2024 15:31:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Your Name
-- Create date: YYYY-MM-DD
-- Description: Data fetch, Insert and Updates to Table - [dbo].[uspTopicTrace]
-- =============================================
ALTER PROCEDURE [dbo].[uspTopicTrace]
    -- Add the parameters for the stored procedure here
    @State INT = 0,
    @FirstTopicName NVARCHAR(100) = NULL,
    @SecondTopicName NVARCHAR(100) = NULL,
    @isFirstTopicCreated INT = NULL,
    @isSecondTopicCreated INT = NULL
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY
		-- Start transaction
        BEGIN TRANSACTION;

        -- CheckExistence
        IF @State = 1
        BEGIN
            SELECT [FirstTopicName],
                   [SecondTopicName],
                   [isFirstTopicCreated],
                   [isSecondTopicCreated]
            FROM [TopicTrace] WITH (NOLOCK);
        END

        -- InsertData
        ELSE IF @State = 2
        BEGIN
            -- Check if a row not exists and perform the operation
            IF NOT EXISTS (SELECT 1 FROM [SpiderETMDB].[dbo].[TopicTrace] WITH (NOLOCK))
            BEGIN
                INSERT INTO [SpiderETMDB].[dbo].[TopicTrace]
                (
                    [FirstTopicName],
                    [SecondTopicName],
                    [isFirstTopicCreated],
                    [isSecondTopicCreated]
                )
                VALUES
                (@FirstTopicName, @SecondTopicName, @isFirstTopicCreated, @isSecondTopicCreated);
            END
        END

        -- UpdateData
        ELSE IF @State = 3
        BEGIN
            -- Check if a row exists and perform update operation
            IF EXISTS (SELECT 1 FROM [SpiderETMDB].[dbo].[TopicTrace] WITH (NOLOCK))
            BEGIN
                UPDATE [SpiderETMDB].[dbo].[TopicTrace]
                SET [FirstTopicName] = @FirstTopicName,
                    [SecondTopicName] = @SecondTopicName,
                    [isFirstTopicCreated] = @isFirstTopicCreated,
                    [isSecondTopicCreated] = @isSecondTopicCreated;
            END
        END

        -- DeleteData
        ELSE IF @State = 4
        BEGIN
            -- Check if a row exists and perform delete operation
            IF EXISTS (SELECT 1 FROM [TopicTrace] WITH (NOLOCK))
            BEGIN
                DELETE FROM TopicTrace;
            END
        END

		-- Commit transaction
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
		-- Handle exceptions and rollback transaction if needed
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Handle exceptions
        SELECT ERROR_NUMBER() AS ErrorNumber,
               ERROR_MESSAGE() AS ErrorMessage,
               ERROR_SEVERITY() AS ErrorSeverity,
               ERROR_STATE() AS ErrorState,
               ERROR_LINE() AS ErrorLine,
               ERROR_PROCEDURE() AS ErrorProcedure;

    END CATCH;

END