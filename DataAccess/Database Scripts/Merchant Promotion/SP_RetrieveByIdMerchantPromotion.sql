CREATE OR ALTER PROCEDURE RET_MERCHANT_PROMOTION_BY_ID_PR
    @P_PromotionID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        PromotionID, MerchantID, PromotionType, DiscountPercentage, MaxRefund, 
        StartDate, EndDate, AvailableQuantity, ValidationStatus, CreatedAt, 
        MerchantPromotionName, MerchantPromotionDescription, MerchantPromotionTerms, 
        MerchantPromotionImage
    FROM 
        MerchantPromotions
    WHERE 
        PromotionID = @P_PromotionID;
END;
GO