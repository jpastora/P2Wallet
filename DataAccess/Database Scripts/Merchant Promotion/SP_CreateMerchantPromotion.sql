CREATE PROCEDURE CREATE_MERCHANT_PROMOTION_PR
    @P_MerchantID INT,
    @P_PromotionType VARCHAR(20),
    @P_DiscountPercentage DECIMAL(5,2),
    @P_MaxRefund DECIMAL(12,2),
    @P_StartDate DATETIME = NULL,
    @P_EndDate DATETIME = NULL,
    @P_AvailableQuantity INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MerchantPromotions (
        MerchantID,
        PromotionType,
        DiscountPercentage,
        MaxRefund,
        StartDate,
        EndDate,
        AvailableQuantity,
        CreatedAt
    )
    VALUES (
        @P_MerchantID,
        @P_PromotionType,
        @P_DiscountPercentage,
        @P_MaxRefund,
        @P_StartDate,
        @P_EndDate,
        @P_AvailableQuantity,
        GETDATE()
    );
END;
GO