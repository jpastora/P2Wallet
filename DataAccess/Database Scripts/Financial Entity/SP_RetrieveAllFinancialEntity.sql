CREATE OR ALTER PROCEDURE RET_ALL_FINANCIAL_ENTITIES_PR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        FinancialEntityID, EntityName, TaxID, LogoImage, -- Columna añadida
        Latitude, Longitude, ContactPhone, Email, 
        CommissionPercentage, ValidationStatus, CreatedAt, UpdatedAt
    FROM FinancialEntities;
END;
GO