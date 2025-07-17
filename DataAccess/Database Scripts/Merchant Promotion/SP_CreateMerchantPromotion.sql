CREATE OR ALTER PROCEDURE CREATE_MERCHANT_PROMOTION_PR
    @P_MerchantID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME,
    @P_EndDate DATETIME,
    @P_AvailableQuantity INT,
    @P_MerchantPromotionName VARCHAR(100),
    @P_MerchantPromotionDescription VARCHAR(255),
    @P_MerchantPromotionTerms VARCHAR(MAX),
    @P_MerchantPromotionImage VARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO MerchantPromotions (
        MerchantID, PromotionType, DiscountPercentage, MaxRefund, 
        StartDate, EndDate, AvailableQuantity, CreatedAt, 
        MerchantPromotionName, MerchantPromotionDescription, MerchantPromotionTerms, 
        MerchantPromotionImage
    )
    VALUES (
        @P_MerchantID, @P_PromotionType, @P_DiscountPercentage, @P_MaxRefund, 
        @P_StartDate, @P_EndDate, @P_AvailableQuantity, GETDATE(), 
        @P_MerchantPromotionName, @P_MerchantPromotionDescription, @P_MerchantPromotionTerms, 
        @P_MerchantPromotionImage
    );
END;
GO