CREATE PROCEDURE CREATE_MERCHANT_PR
    @P_MerchantName VARCHAR(100),
    @P_TaxID VARCHAR(20),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Phone VARCHAR(15),
    @P_Email VARCHAR(100),
    @P_CommissionPercentage DECIMAL(5,2)
AS
BEGIN
    INSERT INTO Merchants (
        CreatedAt, MerchantName, TaxID, Latitude, Longitude,
        Phone, Email, CommissionPercentage
    )
    VALUES (
        GETDATE(), @P_MerchantName, @P_TaxID, @P_Latitude, @P_Longitude,
        @P_Phone, @P_Email, @P_CommissionPercentage
    )
END
GO
