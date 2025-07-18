CREATE OR ALTER PROCEDURE CREATE_FINANCIAL_ENTITY_PR
    @P_EntityName VARCHAR(100),
    @P_TaxID VARCHAR(20),
    @P_LogoImage VARCHAR(300) = NULL, -- Parámetro añadido
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_ContactPhone VARCHAR(15),
    @P_Email VARCHAR(100),
    @P_CommissionPercentage DECIMAL(5,2)
AS
BEGIN
    INSERT INTO FinancialEntities (
        EntityName, TaxID, LogoImage, -- Columna añadida
        Latitude, Longitude, ContactPhone, Email, 
        CommissionPercentage, CreatedAt
    )
    VALUES (
        @P_EntityName, @P_TaxID, @P_LogoImage, -- Valor añadido
        @P_Latitude, @P_Longitude, @P_ContactPhone, @P_Email, 
        @P_CommissionPercentage, GETDATE()
    );
END;
GO