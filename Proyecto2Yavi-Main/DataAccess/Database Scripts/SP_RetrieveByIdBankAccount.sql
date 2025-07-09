CREATE PROCEDURE RET_BANK_ACCOUNT_BY_ID_PR
    @P_BankAccountID INT
AS
BEGIN
    SET NOCOUNT ON;

   SELECT 
        BankAccountID, UserID, IBAN, FinancialEntityID, Status
    FROM
        BankAccounts
    WHERE 
        BankAccountID = @P_BankAccountID
END
GO