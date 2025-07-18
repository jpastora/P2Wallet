CREATE OR ALTER PROCEDURE RET_FINANCIAL_ENTITY_BY_ID_PR
    @P_FinancialEntityID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        FinancialEntityID, EntityName, TaxID, LogoImage, -- Columna añadida
        Latitude, Longitude, ContactPhone, Email, 
        CommissionPercentage, ValidationStatus, CreatedAt, UpdatedAt
    FROM 
        FinancialEntities
    WHERE 
        FinancialEntityID = @P_FinancialEntityID;
END;
GO