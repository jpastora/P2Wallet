CREATE PROCEDURE UPDATE_MERCHANT_PROMOTION_PR
    @P_PromotionID INT,
    @P_MerchantID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME = NULL,
    @P_EndDate DATETIME = NULL,
    @P_AvailableQuantity INT = NULL,
    @P_ValidationStatus VARCHAR(15)
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
        ValidationStatus = @P_ValidationStatus
    WHERE
        PromotionID = @P_PromotionID;
END;
GO

