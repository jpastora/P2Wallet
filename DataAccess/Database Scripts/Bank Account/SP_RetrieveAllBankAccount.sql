CREATE PROCEDURE RET_ALL_BANK_ACCOUNTS_PR
AS
BEGIN
    SELECT 
        BankAccountID,
        UserID,
        IBAN,
        FinancialEntityID,
        ValidationStatus,
        RegisteredAt
    FROM 
        BankAccounts
END
GO
