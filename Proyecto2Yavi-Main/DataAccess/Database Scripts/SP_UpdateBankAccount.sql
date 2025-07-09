CREATE PROCEDURE UPDATE_BANK_ACCOUNT_PR
    @P_BankAccountID INT,
    @P_UserID INT,
    @P_IBAN VARCHAR(22),
    @P_FinancialEntityID INT,
    @P_Status BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        BankAccounts
    SET
        UserID = @P_UserID,
        IBAN = @P_IBAN,
        FinancialEntity = @P_FinancialEntityID,
        Status = @P_Status,
        UpdatedAt = GETDATE()
    WHERE
        BankAccountID = @P_BankAccountID
END
GO