CREATE PROCEDURE RET_ALL_FINANCIAL_PROMOTIONS_PR
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
    FROM FinancialPromotions;
END;
GO