CREATE OR ALTER PROCEDURE CREATE_BANK_ACCOUNT_PR
    @P_UserID INT,
    @P_IBAN CHAR(22),
    @P_FinancialEntityID INT,
    @P_ValidationStatus VARCHAR(15) = 'Active', -- Optional parameter
    @P_Balance DECIMAL(18, 2) = 0.00            -- << New optional parameter
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO BankAccounts (
        UserID,
        IBAN,
        FinancialEntityID,
        RegisteredAt,
        ValidationStatus,
        Balance -- << New column
    )
    VALUES (
        @P_UserID,
        @P_IBAN,
        @P_FinancialEntityID,
        GETDATE(),
        @P_ValidationStatus,
        @P_Balance -- << New value
    );
END;
GO
