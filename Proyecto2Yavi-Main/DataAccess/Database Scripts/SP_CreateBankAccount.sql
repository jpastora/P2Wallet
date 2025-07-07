CREATE PROCEDURE CREATE_BANK_ACCOUNT_PR
    @P_UserID INT,
    @P_IBAN VARCHAR(22),
    @P_FinancialEntityID INT
AS
BEGIN
    INSERT INTO Administrators (
        UserID, 
        IBAN, 
        FinancialEntityID,
        RegisteredAt
    )
    VALUES (
        @P_UserID, 
        @P_IBAN, 
        @P_FinancialEntityID, 
        GETDATE()
    )
END
GO