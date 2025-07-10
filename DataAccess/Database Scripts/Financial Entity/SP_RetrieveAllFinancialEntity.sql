CREATE PROCEDURE RET_ALL_FINANCIAL_ENTITIES_PR
AS
BEGIN
    SELECT 
        FinancialEntityID, CreatedAt, ValidationStatus, 
        EntityName, TaxID, Latitude, Longitude, 
        ContactPhone, Email, CommissionPercentage
    FROM FinancialEntities
END
GO