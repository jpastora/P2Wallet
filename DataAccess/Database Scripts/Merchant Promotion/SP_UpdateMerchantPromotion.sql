CREATE OR ALTER PROCEDURE UPDATE_MERCHANT_PROMOTION_PR
    @P_PromotionID INT,
    @P_MerchantID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME,
    @P_EndDate DATETIME,
    @P_AvailableQuantity INT,
    @P_ValidationStatus VARCHAR(15),
    @P_MerchantPromotionName VARCHAR(100),
    @P_MerchantPromotionDescription VARCHAR(255),
    @P_MerchantPromotionTerms VARCHAR(MAX),
    @P_MerchantPromotionImage VARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MerchantPromotions
    SET
        MerchantID = @P_MerchantID,
        PromotionType = @P_PromotionType,
        DiscountPercentage = @P_DiscountPercentage,
        MaxRefund = @P_MaxRefund,
        StartDate = @P_StartDate,
        EndDate = @P_EndDate,
        AvailableQuantity = @P_AvailableQuantity,
        ValidationStatus = @P_ValidationStatus,
        MerchantPromotionName = @P_MerchantPromotionName,
        MerchantPromotionDescription = @P_MerchantPromotionDescription,
        MerchantPromotionTerms = @P_MerchantPromotionTerms,
        MerchantPromotionImage = @P_MerchantPromotionImage
    WHERE
        PromotionID = @P_PromotionID;
END;
GO