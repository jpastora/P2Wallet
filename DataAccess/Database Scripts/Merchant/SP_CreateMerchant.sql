CREATE PROCEDURE CREATE_MERCHANT_PR
    @P_MerchantName VARCHAR(100),
    @P_TaxID VARCHAR(20),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_ContactPhone VARCHAR(15),
    @P_Email VARCHAR(100),
    @P_CommissionPercentage DECIMAL(5,2)
AS
BEGIN
    INSERT INTO Merchants (
        CreatedAt, MerchantName, TaxID, Latitude, Longitude,
        ContactPhone, Email, CommissionPercentage
    )
    VALUES (
        GETDATE(), @P_MerchantName, @P_TaxID, @P_Latitude, @P_Longitude,
        @P_ContactPhone, @P_Email, @P_CommissionPercentage
    )
END
GO
