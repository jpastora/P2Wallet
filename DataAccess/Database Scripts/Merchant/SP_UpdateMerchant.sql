CREATE PROCEDURE UPDATE_MERCHANT_PR
    @P_MerchantID INT,
    @P_MerchantName VARCHAR(100),
    @P_TaxID VARCHAR(20),
    @P_LogoImage VARCHAR(300),
    @P_Latitude DECIMAL(9,6),
    @P_Longitude DECIMAL(9,6),
    @P_Phone VARCHAR(15),
    @P_Email VARCHAR(100),
    @P_CommissionPercentage DECIMAL(5,2),
    @P_ValidationStatus BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Merchants
    SET
        MerchantName = @P_MerchantName,
        TaxID = @P_TaxID,
        LogoImage = @P_LogoImage,
        Latitude = @P_Latitude,
        Longitude = @P_Longitude,
        Phone = @P_Phone,
        Email = @P_Email,
        CommissionPercentage = @P_CommissionPercentage,
        ValidationStatus = @P_ValidationStatus,
        UpdatedAt = GETDATE()
    WHERE
        MerchantID = @P_MerchantID
END
GO
