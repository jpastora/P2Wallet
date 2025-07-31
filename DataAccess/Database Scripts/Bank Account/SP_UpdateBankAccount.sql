CREATE OR ALTER PROCEDURE UPDATE_BANK_ACCOUNT_PR
    @P_BankAccountID INT,
    @P_UserID INT,
    @P_IBAN CHAR(22),
    @P_FinancialEntityID INT,
    @P_ValidationStatus VARCHAR(15),
    @P_Balance DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE BankAccounts
    SET
        UserID = @P_UserID,
        IBAN = @P_IBAN,
        FinancialEntityID = @P_FinancialEntityID,
        ValidationStatus = @P_ValidationStatus,
        Balance = @P_Balance
    WHERE
        BankAccountID = @P_BankAccountID
END
GO
