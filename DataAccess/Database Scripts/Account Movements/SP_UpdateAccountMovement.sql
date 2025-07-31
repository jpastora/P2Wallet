CREATE OR ALTER PROCEDURE UPDATE_ACCOUNT_MOVEMENT_PR
    @P_MovementID INT,
    @P_TransactionID INT,
    @P_SourceAccountDescription VARCHAR(100),
    @P_DestinationAccountDescription VARCHAR(100),
    @P_MovementType VARCHAR(50),
    @P_Amount DECIMAL(12, 2),
    @P_Timestamp DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AccountMovements
    SET
        TransactionID = @P_TransactionID,
        SourceAccountDescription = @P_SourceAccountDescription,
        DestinationAccountDescription = @P_DestinationAccountDescription,
        MovementType = @P_MovementType,
        Amount = @P_Amount,
        Timestamp = @P_Timestamp
    WHERE
        MovementID = @P_MovementID
END
GO