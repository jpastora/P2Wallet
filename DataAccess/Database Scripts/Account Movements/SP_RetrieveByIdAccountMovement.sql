CREATE OR ALTER PROCEDURE RET_ACCOUNT_MOVEMENT_BY_ID_PR
    @P_MovementID INT
AS
BEGIN
    SET NOCOUNT ON;

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
    WHERE 
        MovementID = @P_MovementID
END
GO