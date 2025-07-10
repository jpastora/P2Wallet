CREATE PROCEDURE CREATE_BANK_ACCOUNT_PR
    @P_UserID INT,
    @P_IBAN CHAR(22),
    @P_FinancialEntityID INT
AS
BEGIN
    INSERT INTO BankAccounts (
        UserID,
        IBAN,
        FinancialEntityID,
        RegisteredAt,
        Status
    )
    VALUES (
        @P_UserID,
        @P_IBAN,
        @P_FinancialEntityID,
        GETDATE(),
        1  
    )
END
GO
