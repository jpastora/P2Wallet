CREATE PROCEDURE RET_ALL_MERCHANTS_PR
AS
BEGIN
    SELECT 
        MerchantID, CreatedAt, ValidationStatus, 
        MerchantName, TaxID, LogoImage, Latitude, Longitude, 
        ContactPhone, Email, CommissionPercentage
    FROM Merchants
END
GO