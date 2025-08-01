CREATE OR ALTER PROCEDURE RET_ALL_BANK_ACCOUNTS_PR
AS
BEGIN
    SELECT 
        BankAccountID,
        UserID,
        IBAN,
        FinancialEntityID,
        ValidationStatus,
        RegisteredAt,
        Balance
    FROM 
        BankAccounts
END
GO
