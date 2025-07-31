CREATE OR ALTER PROCEDURE CREATE_ACCOUNT_MOVEMENT_PR
    @P_TransactionID INT,
    @P_SourceAccountDescription VARCHAR(100),
    @P_DestinationAccountDescription VARCHAR(100),
    @P_MovementType VARCHAR(50),
    @P_Amount DECIMAL(12, 2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AccountMovements
    (
        TransactionID,
        SourceAccountDescription,
        DestinationAccountDescription,
        MovementType,
        Amount,
        Timestamp
    )
    VALUES
    (
        @P_TransactionID,
        @P_SourceAccountDescription,
        @P_DestinationAccountDescription,
        @P_MovementType,
        @P_Amount,
        GETDATE()
    )
END
GO