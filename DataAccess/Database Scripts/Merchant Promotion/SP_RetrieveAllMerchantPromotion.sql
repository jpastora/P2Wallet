CREATE OR ALTER PROCEDURE RET_ALL_MERCHANT_PROMOTIONS_PR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        PromotionID, MerchantID, PromotionType, DiscountPercentage, MaxRefund, 
        StartDate, EndDate, AvailableQuantity, ValidationStatus, CreatedAt, 
        MerchantPromotionName, MerchantPromotionDescription, MerchantPromotionTerms, 
        MerchantPromotionImage
    FROM MerchantPromotions;
END;
GO