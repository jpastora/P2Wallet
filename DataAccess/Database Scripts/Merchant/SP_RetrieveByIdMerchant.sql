CREATE PROCEDURE RET_MERCHANT_BY_ID_PR
    @P_MerchantID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MerchantID,
        MerchantName,
        TaxID,
        LogoImage,
        Latitude,
        Longitude,
        Phone,
        Email,
        CommissionPercentage,
        ValidationStatus,
        CreatedAt,
        UpdatedAt
    FROM 
        Merchants
    WHERE 
        MerchantID = @P_MerchantID
END
GO
