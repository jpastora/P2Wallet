CREATE PROCEDURE UPDATE_FINANCIAL_ENTITY_PR
    @P_FinancialEntityID INT,
    @P_EntityName VARCHAR(100),
    @P_TaxID VARCHAR(20),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_ContactPhone VARCHAR(15),
    @P_Email VARCHAR(100),
    @P_CommissionPercentage DECIMAL(5,2),
    @P_ValidationStatus VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE FinancialEntities
    SET
        EntityName = @P_EntityName,
        TaxID = @P_TaxID,
        Latitude = @P_Latitude,
        Longitude = @P_Longitude,
        ContactPhone = @P_ContactPhone,
        Email = @P_Email,
        CommissionPercentage = @P_CommissionPercentage,
        ValidationStatus = @P_ValidationStatus,
        UpdatedAt = GETDATE()
    WHERE
        FinancialEntityID = @P_FinancialEntityID
END
GO
