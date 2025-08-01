CREATE OR ALTER PROCEDURE RET_ALL_ACCOUNT_MOVEMENTS_PR
AS
BEGIN
    SELECT 
        MovementID,
        TransactionID,
        SourceAccountDescription,
        DestinationAccountDescription,
        MovementType,
        Amount,
        Timestamp
    FROM 
        AccountMovements
END
GO