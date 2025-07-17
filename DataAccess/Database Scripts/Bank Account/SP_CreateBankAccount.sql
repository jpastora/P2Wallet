CREATE OR ALTER PROCEDURE CREATE_BANK_ACCOUNT_PR
    @P_UserID INT,
    @P_IBAN CHAR(22),
    @P_FinancialEntityID INT,
    @P_ValidationStatus VARCHAR(15) = 'Active' -- << PARÁMETRO AÑADIDO (OPCIONAL)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO BankAccounts (
        UserID,
        IBAN,
        FinancialEntityID,
        RegisteredAt,
        ValidationStatus -- << CAMPO AÑADIDO
    )
    VALUES (
        @P_UserID,
        @P_IBAN,
        @P_FinancialEntityID,
        GETDATE(),
        @P_ValidationStatus -- << VALOR DEL PARÁMETRO
    );
END;
GO

ALTER TABLE BankAccounts
ADD CONSTRAINT UQ_BankAccounts_IBAN UNIQUE (IBAN);
GO

PRINT 'Restricción UNIQUE añadida exitosamente a la columna IBAN.';