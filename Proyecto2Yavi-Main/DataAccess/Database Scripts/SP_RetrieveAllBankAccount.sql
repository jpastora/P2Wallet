CREATE PROCEDURE RET_ALL_BANK_ACCOUNTS_PR
AS
BEGIN
    SELECT 
        BankAccountID, UserID, IBAN, FinancialEntityID, Status
    FROM BankAccounts
END
GO