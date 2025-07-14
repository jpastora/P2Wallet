CREATE PROCEDURE RET_FINANCIAL_PROMOTION_BY_ID_PR
    @P_PromotionID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PromotionID,
        FinancialEntityID,
        PromotionType,
        DiscountPercentage,
        MaxRefund,
        StartDate,
        EndDate,
        AvailableQuantity,
        ValidationStatus,
        CreatedAt
    FROM FinancialPromotions
    WHERE PromotionID = @P_PromotionID;
END;
GO
